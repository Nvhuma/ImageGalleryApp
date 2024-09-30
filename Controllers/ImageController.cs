using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Image;
using api.Dtos.ImageDto.Comments;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

[Route("api/image")]
[ApiController]
public class ImageController : ControllerBase
{
    // Dependencies injected through constructor
    private readonly IImageRepository _imageRepo;
    private readonly ApplicationDBContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<ImageController> _logger;

    // Constructor
    public ImageController(ApplicationDBContext context, IImageRepository imageRepo, ILogger<ImageController> logger, UserManager<AppUser> userManager)
    {
        _imageRepo = imageRepo;
        _context = context;
        _logger = logger;
        _userManager = userManager;
    }

    // GET: api/image
    // Retrieves all images based on query parameters
    [HttpGet]
    public async Task<IActionResult> GetAllAsyn([FromQuery] QueryObject query)
    {
        var images = await _imageRepo.GetAllAsync(query);
        var imageDto = images.Select(s => s.ToImageDto()).ToList();
        return Ok(imageDto);
    }

    // GET: api/image/{id:int}
    // Retrieves a specific image by ID
  
  [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var images = await _imageRepo.GetByIdAsync(id);
        if (images == null)
        {
            return NotFound();
        }
        return Ok(images.ToImageDto());
    }


    // PUT: api/image/{id}
    // Updates an existing image

    [HttpPut]
    [Route("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateImageRequestDto updateDto)
    {
        var ImageModel = await _imageRepo.UpdateAsync(id, updateDto);
        if (ImageModel == null)
        {
            return NotFound();
        }
        return Ok(ImageModel.ToImageDto());
    }


    // DELETE: api/image/{id}
    // Deletes a specific image

    [HttpDelete]
    [Route("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id, [FromQuery] string loggedInUserId = null)
    {
        // Extract the user ID from the authenticated user's claims
        var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // Use the user ID from the token or fallback to the logged-inUserId if provided
        if (string.IsNullOrEmpty(userId))
        {
            if (string.IsNullOrEmpty(loggedInUserId))
            {
                // Handle the case where user ID is not found in the claims and not provided in the request
                return Unauthorized("User ID could not be retrieved from the token and no fallback user ID was provided.");
            }
            userId = loggedInUserId;
        }

        // Fetch the image from the repository
        var imageModel = await _imageRepo.GetByIdAsync(id);
        if (imageModel == null)
        {
            return NotFound();
        }

        // Check if the authenticated user is the owner of the image
        if (imageModel.UserId != userId)
        {
            return Unauthorized("You can only delete images that you have uploaded.");
        }

        // If the user is authorized, delete the image
        await _imageRepo.DeleteAysnc(id);
        return NoContent();
    }






    // POST: api/image
    // Creates a new image
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateImageRequestDto ImageDto)
    {
        try
        {
            _logger.LogInformation("Create method called.");

            if (ImageDto == null)
            {
                _logger.LogError("ImageDto is null.");
                return BadRequest("ImageDto cannot be null.");
            }

            // Validate the model state
            if (!ModelState.IsValid)
            {
                _logger.LogError("Model state is invalid.");
                return BadRequest(ModelState);
            }

            // Retrieve the authentication token from the Authorization header
            var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                _logger.LogError("Authorization header is missing or invalid.");
                return Unauthorized("Authorization header is missing or invalid.");
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            _logger.LogInformation($"Token retrieved: {token}");

            // Retrieve the logged-in user's email
            var userEmail = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            _logger.LogInformation($"User email retrieved: {userEmail}");

            // Check if the user email is valid
            if (string.IsNullOrEmpty(userEmail))
            {
                _logger.LogError("User not authenticated or email claim not found.");
                return Unauthorized("User not authenticated or email claim not found.");
            }

            // Find the user by email
            var appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            _logger.LogInformation($"User found: {appUser?.Id}");

            // Check if the user exists
            if (appUser == null)
            {
                _logger.LogError("User not found.");
                return NotFound("User not found.");
            }

            // Validate that critical fields in ImageDto are not null or empty
            if (string.IsNullOrEmpty(ImageDto.Title))
            {
                _logger.LogError("Title cannot be null or empty.");
                return BadRequest("Title cannot be null or empty.");
            }

            // Create the image model and associate it with the user
            var imageModel = new Image
            {
                Title = ImageDto.Title,
                Description = ImageDto.Description,
                CreatedDate = DateTime.UtcNow,
                UserId = appUser.Id, // Associate the image with the user
                ImageURL = ImageDto.ImageURL // Ensure the URL is set
            };

            _logger.LogInformation("Image model created.");

            // Save the image to the database
            await _imageRepo.CreateAsync(imageModel);
            _logger.LogInformation("Image saved to the database.");

            // Return the created image response
            return CreatedAtAction(nameof(GetById), new { Id = imageModel.ImageId }, imageModel.ToImageDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating the image.");
            return StatusCode(500, "Internal server error.");
        }



        // GET: api/image
        // Retrieves all images for the currently logged-in user





    }

    [HttpGet("mylibrary")]
    [Authorize]
    public async Task<IActionResult> GetAllByIdAsyn([FromQuery] QueryObject query)
    {
        var userEmail = User.GetUserEmail();
        var user = await _userManager.FindByEmailAsync(userEmail);
        var images = await _imageRepo.GetAllByUserIdAsync(query, user.Id);
        var imageDto = images.Select(s => s.ToImageDto()).ToList();
        return Ok(imageDto);
    }


}

