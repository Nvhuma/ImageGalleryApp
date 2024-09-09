using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
using api.Data;
using api.Dtos.Account;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using OtpNet;
using QRCoder;
using System;
using Microsoft.AspNetCore.Authorization;



namespace api.Controllers
{
    [Route("api/Account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly ITokenService _tokenService;

        private readonly UserManager<AppUser> _userManager;

        private readonly SignInManager<AppUser> _signinmanager;

        private readonly IEmailService _emailService;

        private readonly IConfiguration _configuration;

        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, SignInManager<AppUser> signinmanager, IEmailService emailService, ILogger<AccountController> logger, IConfiguration configuration, ApplicationDBContext context)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signinmanager = signinmanager;
            _emailService = emailService;
            _logger = logger;
            _configuration = configuration;
            _context = context;

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(loginDto.UserName.ToLower());

            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid username or password" });
            }

            if (!user.EmailConfirmed)
            {
                return Unauthorized(new { Message = "User email is not confirmed." });
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                var lockoutEndDate = await _userManager.GetLockoutEndDateAsync(user);
                var remainingLockoutTime = lockoutEndDate?.Subtract(DateTimeOffset.UtcNow);

                return Unauthorized(new
                {
                    Message = "Account is locked due to multiple failed login attempts.",
                    RemainingLockoutTime = remainingLockoutTime?.TotalSeconds
                });
            }

            var result = await _signinmanager.CheckPasswordSignInAsync(user, loginDto.Password, true);

            if (!result.Succeeded)
            {
                await _userManager.AccessFailedAsync(user);

                var accessFailedCount = await _userManager.GetAccessFailedCountAsync(user);
                var maxFailedAccessAttempts = 3;
                var attemptsLeft = maxFailedAccessAttempts - accessFailedCount;

                if (accessFailedCount >= maxFailedAccessAttempts)
                {
                    await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddMinutes(5));

                    return Unauthorized(new
                    {
                        Message = "Account is locked due to multiple failed login attempts.",
                        RemainingLockoutTime = 120 // 3 minutes lockout in seconds
                    });
                }

                return Unauthorized(new { Message = $"Invalid username or password. You have {attemptsLeft} attempt(s) left." });
            }

            // Login successful, return a response indicating success, 
            // no need to return a token yet, since TOTP needs to be verified
            return Ok(new
            {
                UserName = user.UserName,
                EmailAddress = user.Email,
                Message = "Username and password are correct, proceed to TOTP verification."
            });
        }


        [HttpPost("totp")]
        public async Task<IActionResult> VerifyTotp(TotpDto totpDto)
        {
            var user = await _userManager.FindByNameAsync(totpDto.UserName.ToLower());

            if (user == null || !await _userManager.CheckPasswordAsync(user, totpDto.Password))
            {
                return Unauthorized(new { Message = "Invalid username or password" });
            }
            // relies on the TOTP algorythm's inherent behavior and the   VerificationWindow
            var totp = new Totp(Base32Encoding.ToBytes(user.TotpSecret));
            if (!totp.VerifyTotp(totpDto.TotpCode, out long timeStepMatched, new VerificationWindow(2, 2)))
            {
                return Unauthorized(new { Message = "Invalid TOTP code" });
            }

            // Reset the access failed count after a successful login
            await _userManager.ResetAccessFailedCountAsync(user);

            // Generate and return the token
            var token = _tokenService.CreateToken(user);
            return Ok(new
            {
                UserName = user.UserName,
                token,
                UserId = user.Id,
                EmailAddress = user.Email
            });
              // new VerificationWindow(2, 2) when calling totp.VerifyTotp. 
      //This means that the TOTP code will be accepted if it is valid within a 2-time-step window before or after the current time. 
      // this provides a small buffer of time (usually 30 seconds per time step) 
      //the code could be valid for up to 1 minute (2 steps before and 2 steps after the current time)
        }
     


        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUserByEmail = await _userManager.FindByEmailAsync(registerDto.EmailAddress);
            var existingUserByUsername = await _userManager.FindByNameAsync(registerDto.UserName);

            if (existingUserByEmail != null)
            {
                return BadRequest(new { Message = "A user with this email address already exists." });
            }

            if (existingUserByUsername != null)
            {
                return BadRequest(new { Message = "A user with this username already exists." });
            }

            var appUser = new AppUser
            {
                UserName = registerDto.UserName,
                Email = registerDto.EmailAddress,
                Names = registerDto.Names,
            };

            // Generate the TOTP secret key
            var key = KeyGeneration.GenerateRandomKey(20);
            var base32Secret = Base32Encoding.ToString(key);
            appUser.TotpSecret = base32Secret;

            var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);

            if (createdUser.Succeeded)
            {
                var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
                var encodedToken = HttpUtility.UrlEncode(emailConfirmationToken);
                var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = appUser.Id, token = emailConfirmationToken }, Request.Scheme);

                var qrGenerator = new QRCodeGenerator();
                var otpUri = $"otpauth://totp/Image Gallery App:{appUser.Email}?secret={base32Secret}&issuer=Image Gallery App";
                var qrCodeData = qrGenerator.CreateQrCode(otpUri, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new Base64QRCode(qrCodeData).GetGraphic(20);
                var emailBody = $@"
                  <html>
                 <body>

            <h1> Dear {appUser.UserName}, </h1>
            
          <p>  This email is to confirm that your account has been successfully registered with the Image Gallery Ease.
            Please confirm your account by clicking <a href='{confirmationLink}'>here</a>. </p>

            If you did not initiate this registration, please ignore this email.

            The information contained in this communication is confidential and may be legally privileged. It is intended solely for use by the originator and others authorised to receive it. If you are not that person you are hereby notified that any disclosure, copying, distribution or taking action in reliance of the contents of this information is strictly prohibited and may be unlawful. Neither Singular Systems (Pty) Ltd (Registration 2002/001492/07) nor any of its subsidiaries are liable for the proper, complete transmission of the information contained in this communication, or for any delay in its receipt, or for the assurance that it is virus-free. If you have received this in error please report it to: sis@singular.co.za

            Best regards,
            Singular Systems Team
        ";

                await _emailService.SendEmailAsync(appUser.Email, "Confirm Your Account", emailBody);



                return Ok(new
                {
                    UserName = appUser.UserName,
                    EmailAddress = appUser.Email,
                    token = _tokenService.CreateToken(appUser),
                    qrCode,
                    base32Secret
                });
            }
            else
            {
                return StatusCode(500, createdUser.Errors);
            }
        }


// [HttpPost("forgot-password")]
// public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
// {
//     if (!ModelState.IsValid)
//         return BadRequest(ModelState);

//     var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
//     if (user == null) 
//         return NotFound("User with email does not exist");

//     var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
//     var resetLink = $"http://localhost:5173/reset-password?Email={forgotPasswordDto.Email}&token={Uri.EscapeDataString(resetToken)}";

//     var emailBody = $@"
//     <html>
//     <body>
//         <h1>Dear {user.UserName},</h1>
//         <p>We received a request to reset your password for your account with Gallery Ease.</p>
//         <p>Please reset your password by clicking <a href='{resetLink}'>here</a>.</p>
//         <p>If you did not request a password reset, please ignore this email.</p>
//         <p>The information contained in this communication is confidential and may be legally privileged. It is intended solely for use by the originator and others authorised to receive it. If you are not that person you are hereby notified that any disclosure, copying, distribution or taking action in reliance of the contents of this information is strictly prohibited and may be unlawful. Neither Singular Systems (Pty) Ltd (Registration 2002/001492/07) nor any of its subsidiaries are liable for the proper, complete transmission of the information contained in this communication, or for any delay in its receipt, or for the assurance that it is virus-free. If you have received this in error please report it to: sis@singular.co.za</p>
//         <p>Best regards,<br/>Gallery Ease Team</p>
//     </body>
//     </html>";

//     // Send the email with the reset link
//     await _emailService.SendEmailAsync(user.Email, "Password Reset Request for Gallery Ease", emailBody);

//     // Return the reset link to the frontend in the response
//     return Ok(new { message = "Password reset link has been sent to your email.", resetLink = resetLink });
// }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null) return NotFound("User with email does not exist");

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = $"http://localhost:5173/reset-password?Email={forgotPasswordDto.Email}&token={Uri.EscapeDataString(resetToken)}";

            var emailBody = $@"   
             <html>
        <body>
           <h1> Dear {user.UserName}, </h1>
                                  
        
      <p>  We received a request to reset your password for your account with Gallery Ease. </p>
        <p>  Please reset your password by clicking <a href='{resetLink}'>here</a>. </p>

        If you did not request a password reset, please ignore this email.

        The information contained in this communication is confidential and may be legally privileged. It is intended solely for use by the originator and others authorised to receive it. If you are not that person you are hereby notified that any disclosure, copying, distribution or taking action in reliance of the contents of this information is strictly prohibited and may be unlawful. Neither Singular Systems (Pty) Ltd (Registration 2002/001492/07) nor any of its subsidiaries are liable for the proper, complete transmission of the information contained in this communication, or for any delay in its receipt, or for the assurance that it is virus-free. If you have received this in error please report it to: sis@singular.co.za


        Best regards,
        Gallery Ease Team

         </body>
        </html>
    ";

            await _emailService.SendEmailAsync(user.Email, "Password Reset Request for Gallery Ease", emailBody);

            

             return Ok(new { message = "Password reset link has been sent to your email.", resetLink = resetLink });

           
        }

        private string GenerateResetLink(string userId, string token)
        {
            //  generate a reset link
            //  a link to a page on your frontend that accepts the token and allows password reset

            return $"http://localhost:5173/reset-password?userId={userId}&token={Uri.EscapeDataString(token)}";


        }

        private async Task SendResetLinkEmail(string email, string resetLink)
        {

            //  using  service .NET's built-in SmtpClient


            var smtpHost = _configuration["Smtp:Host"];
            var smtpPortString = int.Parse(_configuration["Smtp:Port"]);
            var smtpUsername = _configuration["Smtp:Username"];
            var smtpPassword = _configuration["Smtp:Password"];
            var SmtpFrom = _configuration["Smtp:FromAddress"];


            var SmtpClient = new SmtpClient(smtpHost)
            {
                Port = 587,
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(SmtpFrom),
                Subject = "Password Reset Request",
                Body = $" Please reset your password by clicking on this link: {resetLink}",
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);

            try
            {
                await SmtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent successfully to {Email}", email);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not send email to {Email}", email);
                // Log the exception
                throw new InvalidOperationException("Could not send email", ex);

            }
        }

        [HttpGet("confirmemail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return BadRequest("User Id and Token are required");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Ok("Email confirmed successfully!");
            }
            else
            {
                return StatusCode(500, "Email confirmation failed");
            }
        }



        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Sign out the user
                await _signinmanager.SignOutAsync();
                // Additional token revocation logic if needed
                return Ok(new { message = "User logged out successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout failed");
                return StatusCode(500, new { message = "Logout failed." });
            }
        }

    


   
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                return BadRequest(new { message = "Invalid email address." });
            }

            if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmPassword)
            {
                return BadRequest(new { message = "The new password and confirmation password do not match." });
            }

            // Check if the new password is the same as any previous passwords
            var passwordHistory = await _context.UserPasswordHistory
                                                .Where(ph => ph.AppUserID == user.Id)
                                                .ToListAsync();

            foreach (var oldPassword in passwordHistory)
            {
                var passwordVerificationResult = _userManager.PasswordHasher.VerifyHashedPassword(user, oldPassword.PasswordHash, resetPasswordDto.NewPassword);
                if (passwordVerificationResult == PasswordVerificationResult.Success)
                {
                    return BadRequest(new { message = "The new password must be different from any of the previous passwords." });
                }
            }

            // Save the old password hash to the password history table
            var oldPasswordHash = user.PasswordHash;
            var newPasswordHistory = new UserPasswordHistory
            {
                PasswordHash = oldPasswordHash,
                AppUserID = user.Id,
                CreateDate = DateTime.UtcNow // Ensure you have a timestamp for password history
            };
            _context.UserPasswordHistory.Add(newPasswordHistory);
            await _context.SaveChangesAsync();

            var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            if (!resetPassResult.Succeeded)
            {
                var errors = resetPassResult.Errors.Select(e => e.Description);
                return BadRequest(new { message = "Password reset failed.", errors });
            }

            return Ok(new { message = "Password has been reset successfully." });
        }

    }





}



