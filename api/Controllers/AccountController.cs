using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
using api.Dtos.Account;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/Account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signinmanager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, SignInManager<AppUser> signinmanager, IEmailService emailService, ILogger<AccountController> logger, IConfiguration configuration)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signinmanager = signinmanager;
            _emailService = emailService;
            _logger = logger;
            _configuration = configuration;

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByNameAsync(loginDto.UserName.ToLower());

            if (user == null) return Unauthorized("Invalid username or password");

            // Check if account is locked
            if (await _userManager.IsLockedOutAsync(user))
            {
                var lockoutEndDate = await _userManager.GetLockoutEndDateAsync(user);
                var remainingLockoutTime = lockoutEndDate?.Subtract(DateTimeOffset.UtcNow);

                return Unauthorized(new
                {
                    Message = "Account is locked due to multiple failed login attempts.",
                    LockoutEnd = lockoutEndDate?.UtcDateTime,
                    RemainingLockoutTime = remainingLockoutTime?.TotalSeconds
                });
            }

            var result = await _signinmanager.CheckPasswordSignInAsync(user, loginDto.Password, true);

            if (!result.Succeeded)
            {
                // Increment access failed count
                await _userManager.AccessFailedAsync(user);

                // Check if the account is now locked
                if (await _userManager.IsLockedOutAsync(user))
                {
                    var lockoutEndDate = await _userManager.GetLockoutEndDateAsync(user);
                    var remainingLockoutTime = lockoutEndDate?.Subtract(DateTimeOffset.UtcNow);

                    return Unauthorized(new
                    {
                        Message = "Account is locked due to multiple failed login attempts.",
                        LockoutEnd = lockoutEndDate?.UtcDateTime,
                        RemainingLockoutTime = remainingLockoutTime?.TotalSeconds
                    });
                }

                return Unauthorized("Invalid username or password");
            }

            // Reset access failed count on successful login
            await _userManager.ResetAccessFailedCountAsync(user);

            return Ok(
                new NewUserDto
                {
                    UserName = user.UserName,
                    EmailAddress = user.Email,
                    token = _tokenService.CreateToken(user)
                }
            );
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Check if a user with the given email or username already exists
                var existingUserByEmail = await _userManager.FindByEmailAsync(registerDto.EmailAddress);
                var existingUserByUsername = await _userManager.FindByNameAsync(registerDto.UserName);

                if (existingUserByEmail != null)
                {
                    return BadRequest("A user with this email address already exists.");
                }

                if (existingUserByUsername != null)
                {
                    return BadRequest("A user with this username already exists.");
                }

                var appUser = new AppUser
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.EmailAddress,
                    Names = registerDto.Names,
                };

                var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);

                if (createdUser.Succeeded)
                {
                    // Generate email confirmation token
                    var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
                    var encodedToken = HttpUtility.UrlEncode(emailConfirmationToken);
                    // Create the confirmation link
                    var confirmationLink = Url.Action("ConfirmEmail", "Account",
                        new { userId = appUser.Id, token = emailConfirmationToken }, Request.Scheme);

                    // Send the confirmation email
                    // (Assuming _emailSender is an instance of a service to send emails)
                    var emailContent = $@"
                    Please confirm your account by clicking this link: <a href='{confirmationLink}'>Confirm Email</a><br /><br />
                    The information contained in this communication is confidential and may be legally privileged. It is intended solely for use by the originator and others authorised to receive it. If you are not that person you are hereby notified that any disclosure, copying, distribution or taking action in reliance of the contents of this information is strictly prohibited and may be unlawful. Neither Singular Systems (Pty) Ltd (Registration 2002/001492/07) nor any of its subsidiaries are liable for the proper, complete transmission of the information contained in this communication, or for any delay in its receipt, or for the assurance that it is virus-free. If you have received this in error please report it to: sis@singular.co.za, IT";

                    await _emailService.SendEmailAsync(appUser.Email, "Confirm your email",
                        $"Please confirm your account by clicking this link: <a href='{confirmationLink}'>link</a>");

                    // Create the token
                    var token = _tokenService.CreateToken(appUser);

                    var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
                    if (roleResult.Succeeded)
                    {
                        var AccessToken = _tokenService.CreateToken(appUser);

                        // Populate AspNetUserTokens table
                        var result = await _userManager.SetAuthenticationTokenAsync(
                            appUser,
                            "TokenProvider", //  can  Replace with token provider name
                            "AccessToken", //   can use any name for the token
                            token
                        );

                        if (result.Succeeded)
                        {
                            return Ok(
                                new NewUserDto
                                {
                                    UserName = appUser.UserName,
                                    EmailAddress = appUser.Email,
                                    token = _tokenService.CreateToken(appUser)
                                }
                            );
                        }
                        else
                        {
                            return StatusCode(500, "Fallied to save token");
                        }

                    }
                    else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else
                {
                    return StatusCode(500, createdUser.Errors);
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
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

            await _emailService.SendEmailAsync(user.Email, "Reset Password Image Gallery", $" Dear  Please reset your password by clicking on this link: {resetLink}");

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
