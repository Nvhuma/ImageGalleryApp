using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Account;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/Account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        // Private fields for dependency injection
        private readonly ITokenService _tokenService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signinmanager;

        // Constructor with dependency injection
        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, SignInManager<AppUser> signinmanager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signinmanager = signinmanager;
        }

        // HTTP POST endpoint for user login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Find the user by username
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.UserName.ToLower());

            // If user not found, return unauthorized
            if (user == null) return Unauthorized("Invalid username!");

            // Check the password
            var result = await _signinmanager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            // If password check failed, return unauthorized
            if (!result.Succeeded) return Unauthorized("USERNAME NOT FOUND AND/OR PASSWORD INCORRECT!");

            // If login successful, return user details and token
            return Ok(
                new NewUserDto
                {
                    UserName  = user.UserName,
                    EmailAddress = user.Email,
                    token = _tokenService.CreateToken(user)
                }
            );
        }

        // HTTP POST endpoint for user registration
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                // Check if the model state is valid
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Create a new AppUser object
                var appUser = new AppUser
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.EmailAddress,
                    Names = registerDto.Names,
                };

                // Attempt to create the user
                var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);

                if (createdUser.Succeeded)
                {
                    // If user creation succeeded, add the user to the "User" role
                    var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
                    if (roleResult.Succeeded)
                    {
                        // If role assignment succeeded, return user details and token
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
                        // If role assignment failed, return 500 status code with errors
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else
                {
                    // If user creation failed, return 500 status code with errors
                    return StatusCode(500, createdUser.Errors);
                }
            }
            catch (Exception e)
            {
                // If any exception occurs, return 500 status code with the exception
                return StatusCode(500, e);
            }
        }
    }
}