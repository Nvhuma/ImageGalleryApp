using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Dtos.ImageDto.Comments;
using api.Extensions;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/comment")]
    public class CommentController : ControllerBase
    {
        // Dependency injection for repositories and user manager
        private readonly ICommentRepository _commentRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly IImageRepository _imageRepo;

        // Constructor
        public CommentController(ICommentRepository commentRepo, UserManager<AppUser> userManager, IImageRepository imageRepo)
        {
            _commentRepo = commentRepo;
            _userManager = userManager;
            _imageRepo = imageRepo;
        }
        

        // GET: api/comment
        // Retrieves all comments
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllAysnc()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var comments = await _commentRepo.GetAllAsync();
            var CommentDto = comments.Select(s => s.ToCommentDto());

            return Ok(CommentDto);
        }

        // GET: api/comment/{id}
        // Retrieves a specific comment by ID
        [HttpGet("{id:int}")]
        public async Task<IActionResult> getbyId([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var comment = await _commentRepo.GetByIdAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment.ToCommentDto());
        }

        // POST: api/comment/{ImageId}
        // Creates a new comment for a specific image
        // [HttpPost("{ImageId:int}")]
        // public async Task<IActionResult> Create([FromRoute] int ImageId, CreateCommentDto commentDto)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }

        //     if (!await _commentRepo.ImageExist(ImageId))
        //     {
        //         return BadRequest("Image does not exist");
        //     }

        //     // Get username and ensure it's not null
        //     var username = User.GetUsername();
        //     if (string.IsNullOrEmpty(username))
        //     {
        //         return BadRequest("User not authenticated or username claim not found");
        //     }

        //     // Find the user by username
        //     var appUser = await _userManager.FindByNameAsync(username);
        //     if (appUser == null)
        //     {
        //         return BadRequest("User not found");
        //     }

        //     // Convert DTO to comment model and set UserId
        //     var commentModel = commentDto.ToCommentFromCreate(ImageId);
        //     commentModel.UserId = appUser.Id;

        //     // Create the comment asynchronously
        //     await _commentRepo.CreateAysnc(commentModel);

        //     // Return the created comment with the appropriate response
        //     return CreatedAtAction(nameof(getbyId), new { id = commentModel.CommentId }, commentModel.ToCommentDto());
        // }
        // POST: api/comment/{ImageId}
        // Creates a new comment for a specific image
        // [HttpPost("{ImageId:int}")]
        // public async Task<IActionResult> Create([FromRoute] int ImageId, CreateCommentDto commentDto)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }

        //     if (!await _commentRepo.ImageExist(ImageId))
        //     {
        //         return BadRequest("Image does not exist");
        //     }

        //     var userEmail = User.GetUserEmail();

        //     if (string.IsNullOrEmpty(userEmail))
        //     {
        //         return BadRequest("User not authenticated or email claim not found");
        //     }

        //     var appUser = await _userManager.FindByEmailAsync(userEmail);

        //     if (appUser == null)
        //     {
        //         return BadRequest("User not found");
        //     }

        //     var commentModel = commentDto.ToCommentFromCreate(ImageId);
        //     commentModel.UserId = appUser.Id;

        //     await _commentRepo.CreateAysnc(commentModel);

        //     return CreatedAtAction(nameof(getbyId), new { id = commentModel.CommentId }, commentModel.ToCommentDto());

  
[HttpPost("{ImageId:int}")]
public async Task<IActionResult> Create([FromRoute] int ImageId, [FromQuery] string Content)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    // Check if the image exists
    if (!await _commentRepo.ImageExist(ImageId))
    {
        return BadRequest("Image does not exist");
    }

    // Retrieve the logged-in user's email from claims
    var userEmail = User.GetUserEmail();

    if (string.IsNullOrEmpty(userEmail))
    {
        return Unauthorized("User not authenticated or email claim not found");
    }

    // Fetch the user by email
    var appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

    if (appUser == null)
    {
        return BadRequest("User not found");
    }

    // Create a comment model and associate the logged-in user's ID
    var commentModel = new Comment
    {
        ImageId = ImageId,
        UserId = appUser.Id,  // Automatically set the UserId from the logged-in user
        Content = Content,    // Use the Content passed in the query
        //CreatedAt = DateTime.UtcNow
    };

    // Save the comment in the repository
    await _commentRepo.CreateAysnc(commentModel);

    // Return a 201 Created response with the comment details
    return CreatedAtAction(nameof(getbyId), new { id = commentModel.CommentId }, commentModel.ToCommentDto());
}

  
        
        // PUT: api/comment/{id}
        // Updates an existing comment
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCommentRequestDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var comment = await _commentRepo.UpdateAsync(id, updateDto.ToCommentFromUpdate());

            if (comment == null)
            {
                return NotFound("comment not found");
            }

            return Ok(comment.ToCommentDto());
        }

        // DELETE: api/comment/{id}
        // Deletes a specific comment
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var commentModel = await _commentRepo.DeleteAsync(id);

            if (commentModel == null)
            {
                return NotFound("Comments does not exist");
            }

            return Ok(commentModel);
        }
    }
}