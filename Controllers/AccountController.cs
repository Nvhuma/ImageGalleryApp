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
using OtpNet;
using QRCoder;

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
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByNameAsync(loginDto.UserName.ToLower());

            if (user == null) return Unauthorized(new { Message = "Invalid username or password" });

            if (!user.EmailConfirmed) return Unauthorized(new { Message = "User email is not confirmed." });

            if (await _userManager.IsLockedOutAsync(user))
            {
                var lockoutEndDate = await _userManager.GetLockoutEndDateAsync(user);
                var remainingLockoutTime = lockoutEndDate?.Subtract(DateTimeOffset.UtcNow);

                return Unauthorized(new
                {
                    Message = "Account is locked due to multiple failed login attempts.",
                    remainingLockoutTime = remainingLockoutTime?.TotalSeconds
                });
            }

            var result = await _signinmanager.CheckPasswordSignInAsync(user, loginDto.Password, true);

            if (!result.Succeeded)
            {
                await _userManager.AccessFailedAsync(user);

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

                return Unauthorized(new { Message = "Invalid username or password" });
            }

            // Verify TOTP code
            var totp = new Totp(Base32Encoding.ToBytes(user.TotpSecret));
            if (!totp.VerifyTotp(loginDto.TotpCode, out long timeStepMatched, new VerificationWindow(2, 2)))
            {
                return Unauthorized(new { Message = "Invalid TOTP code" });
            }

            // Update TwoFactorEnabled column after successful TOTP verification
            if (!user.TwoFactorEnabled)
            {
                user.TwoFactorEnabled = true;
                await _userManager.UpdateAsync(user);
            }

            await _userManager.ResetAccessFailedCountAsync(user);

            return Ok(new
            {
                UserName = user.UserName,
                EmailAddress = user.Email,
                token = _tokenService.CreateToken(user)
            });
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

                await _emailService.SendEmailAsync(appUser.Email, "Confirm Your Account", $"Dear {appUser.UserName}, please confirm your account by clicking <a href='{confirmationLink}'>here</a>.");

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



        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null) return NotFound("User with email does not exist");

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            // var resetLink = Url.Action("ResetPassword", "Account", new{token = resetToken, email = forgotPasswordDto.Email}, Request.Scheme);
            var resetLink = $"http://localhost:5173/reset-password?Email={forgotPasswordDto.Email}&token={Uri.EscapeDataString(resetToken)}";

            await _emailService.SendEmailAsync(user.Email, "Password Reset Request for Gallery Ease", $" Dear {user.UserName} Please reset your password by clicking on this link: {resetLink}");

            return Ok("Password reset link has been sent to your email.");
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
                Body = $" Dear Please reset your password by clicking on this link: {resetLink}",
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
