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

         
        // Constructor
        public ImageController(ApplicationDBContext context, IImageRepository imageRepo)
        {
            _imageRepo = imageRepo;
            _context = context;
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

        // GET: api/image/{id}
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

        // POST: api/image
        // Creates a new image
        // [HttpPost]
        // public async Task<IActionResult> Create([FromForm] CreateImageRequestDto ImageDto)
        // {
        //     var ImageModel = ImageDto.ToImageFromCreateDTO();
        //     var imageModel = new Image
        //     {
        //         Title = ImageDto.Title,
        //         Description = ImageDto.Description,
        //         CreatedDate = DateTime.UtcNow,
        //        // UserId = ImageDto.UserId,
        //         ImageURL = ImageDto.ImageURL // Ensuring that the URL is set
        //     };
        //     await _imageRepo.CreateAsync(ImageModel);
        //     return CreatedAtAction(nameof(GetById), new { Id = ImageModel.ImageId}, ImageModel.ToImageDto());
        // }


    [HttpPost]
public async Task<IActionResult> Create([FromForm] CreateImageRequestDto ImageDto)
{
    // Validate the model state
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    // Retrieve the logged-in user's email
    var userEmail = User.GetUserEmail();

    // Check if the user email is valid
    if (string.IsNullOrEmpty(userEmail))
    {
        return BadRequest("User not authenticated or email claim not found.");
    }

    // Find the user by email
    var appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

    // Check if the user exists
    if (appUser == null)
    {
        return BadRequest("User not found.");
    }

    // Validate that critical fields in ImageDto are not null or empty
    if (string.IsNullOrEmpty(ImageDto.Title))
    {
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

    // Save the image to the database
    await _imageRepo.CreateAsync(imageModel);

    // Return the created image response
    return CreatedAtAction(nameof(GetById), new { Id = imageModel.ImageId }, imageModel.ToImageDto());
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
    }
}