using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
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

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.UserName.ToLower());

            if (user == null) return Unauthorized("Invalid username!");

            var result = await _signinmanager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded) return Unauthorized("USERNAME NOT FOUND AND/OR PASSWORD INCORRECT!");

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

                var appUser = new AppUser
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.EmailAddress,
                    Names = registerDto.Names,
                };

                var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);

                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
                    if (roleResult.Succeeded)
                    {
                        var token = _tokenService.CreateToken(appUser);

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
            
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if(user == null) return NotFound("User with email does not exist");

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
           // var resetLink = Url.Action("ResetPassword", "Account", new{token = resetToken, email = forgotPasswordDto.Email}, Request.Scheme);
          var  resetLink = $"http://localhost:5173/reset-password?Email={forgotPasswordDto.Email}&token={Uri.EscapeDataString(resetToken)}";

            await _emailService.SendEmailAsync(user.Email, "Reset Password Image Gallery", $"Please reset your password by clicking on this link: {resetLink}");

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
                    Body = $"Please reset your password by clicking on this link: {resetLink}",
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






        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                return BadRequest(new { message = "Invalid email address." });
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
