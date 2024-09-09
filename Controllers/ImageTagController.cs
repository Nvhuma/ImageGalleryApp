using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using api.Mappers;
using api.Dtos.ImageTag;
using api.Interfaces;
using api.Helpers;
using api.Models;


namespace api.Controllers
{
    [Route("api/ImageTag")]
    [ApiController]
    public class ImageTagController : ControllerBase
    {
        // Dependencies injected through constructor
        private readonly ApplicationDBContext _context;
        private readonly IImageTagRepository _imageTagRepo;


        private readonly ITagRepository _tagRepository;

        // Constructor
        public ImageTagController(ApplicationDBContext context, IImageTagRepository imageTagRepo,  ITagRepository tagRepository)
        {
            _imageTagRepo = imageTagRepo;
            _context = context;
             _tagRepository = tagRepository;
        }

         // POST: api/ImageTag
        [HttpPost]
        public async Task<IActionResult> CreateImageTag([FromBody] ImageTag imageTag)
        {
            // Ensure that the ImageTagID is not manually set by clients
            imageTag.ImageTagID = 0;

            var createdImageTag = await _imageTagRepo.CreateAsync(imageTag);

            return CreatedAtAction(nameof(GetById), new { id = createdImageTag.ImageTagID }, createdImageTag);
        }

    
         // GET: api/ImageTag/Search/{tagName}
        [HttpGet("Search/{tagName}")]
        public async Task<IActionResult> SearchByTagName(string tagName)
        {
            var images = await _imageTagRepo.GetImagesByTagNameAsync(tagName);
            if (images == null || !images.Any())
            {
                return NotFound();
            }
            return Ok(images);
        }

        // POST: api/ImageTag/{Id}
        // Creates a new image tag
        [HttpPost]
        [Route("{Id}")]
        public async Task<IActionResult> Create(int ImageTagId, [FromBody] CreateImageTagRequestDto imageTagDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var imageTagModel = imageTagDto.ToImageTagFromCreateDTO();
            await _imageTagRepo.CreateAsync(imageTagModel);

            return CreatedAtAction(nameof(GetById), new { id = imageTagModel.ImageTagID }, imageTagModel.ToImageTagDto());
        }

        private object GetById()
        {
            throw new NotImplementedException();
        }

        // PUT: api/ImageTag/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateImageTag(int id, [FromBody] UpdateImageTagRequestDto imageTagDto)
        {
            var updatedImageTag = await _imageTagRepo.UpdateAsync(id, imageTagDto);
            if (updatedImageTag == null)
            {
                return NotFound();
            }
            return Ok(updatedImageTag);
        }

         // DELETE: api/ImageTag/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImageTag(int id)
        {
            var deletedImageTag = await _imageTagRepo.DeleteAsync(id);
            if (deletedImageTag == null)
            {
                return NotFound();
            }
            return Ok(deletedImageTag);
        }
    }
}