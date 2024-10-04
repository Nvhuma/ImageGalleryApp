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

		private readonly ICommentRepository _commentRepo;
		private readonly UserManager<AppUser> _userManager;
		private readonly IImageRepository _imageRepo;


		public CommentController(ICommentRepository commentRepo, UserManager<AppUser> userManager, IImageRepository imageRepo)
		{
			_commentRepo = commentRepo;
			_userManager = userManager;
			_imageRepo = imageRepo;
		}



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


		[HttpGet("{id:int}")]

		[Authorize]
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




		[HttpPost("{ImageId:int}")]
		[Authorize]
		public async Task<IActionResult> Create([FromRoute] int ImageId, [FromBody] CreateCommentDto commentDto)
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
			var user = await _userManager.FindByEmailAsync(userEmail);

			if (user == null)
			{
				return Unauthorized("User not logged in.");
			}

			// Create a comment model and associate the logged-in user's ID
			var commentModel = new Comment
			{
				ImageId = ImageId,
				UserId = user.Id,  // Automatically set the UserId from the logged-in user
				Content = commentDto.Content // Use the content from the DTO
			};

			// Save the comment in the repository
			await _commentRepo.CreateAysnc(commentModel);

			// Return a 201 Created response with the comment details
			return CreatedAtAction(nameof(getbyId), new { id = commentModel.CommentId }, commentModel.ToCommentDto());
		}





		[HttpPut]
		[Route("{id:int}")]

		[Authorize]
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


		[HttpDelete]
		[Route("{id:int}")]

		[Authorize]
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




		[HttpGet("image/{imageId:int}")]
		[Authorize]
		public async Task<IActionResult> GetCommentsByImageId([FromRoute] int imageId)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			// Check if the image exists
			if (!await _commentRepo.ImageExist(imageId))
			{
				return NotFound("Image does not exist");
			}

			// Get comments for the specified image
			var comments = await _commentRepo.GetCommentsByImageIdAsync(imageId);

			if (comments == null || !comments.Any())
			{
				return NotFound("No comments found for this image");
			}

			var commentDtos = comments.Select(c => c.ToCommentDto());

			return Ok(commentDtos);
		}

	}
}
