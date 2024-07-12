using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Image;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
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
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var ImageModel = await _imageRepo.DeleteAysnc(id);
            if (ImageModel == null)
            {
                return NotFound();
            }
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

                // Retrieve the logged-in user's email
                var userEmail = User?.GetUserEmail();
                _logger.LogInformation($"User email retrieved: {userEmail}");

                // Check if the user email is valid
                if (string.IsNullOrEmpty(userEmail))
                {
                    _logger.LogError("User not authenticated or email claim not found.");
                    return BadRequest("User not authenticated or email claim not found.");
                }

                // Find the user by email
                var appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
                _logger.LogInformation($"User found: {appUser?.Id}");

                // Check if the user exists
                if (appUser == null)
                {
                    _logger.LogError("User not found.");
                    return BadRequest("User not found.");
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
        }
    }
}
