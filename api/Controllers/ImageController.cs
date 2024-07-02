using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Image;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/image")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageRepository _imageRepo;
        private readonly ApplicationDBContext _context;
        public ImageController(ApplicationDBContext context, IImageRepository imageRepo)
        {
            _imageRepo = imageRepo;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsyn([FromQuery] QueryObject query)
        {
            var images = await _imageRepo.GetAllAsync(query);

            var imageDto = images.Select( s => s.ToImageDto()).ToList();

            return Ok(imageDto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            var images = await _imageRepo.GetByIdAsync(id);

            if(images == null)
            {
                return NotFound();
            }
           
           return Ok(images.ToImageDto());
        }
        
        [HttpPost]
        public  async Task<IActionResult> Create([FromForm]  CreateImageRequestDto  ImageDto)
        {
            var ImageModel = ImageDto.ToImageFromCreateDTO();
            var imageModel = new Image
           {
                    Title = ImageDto.Title,
                    Description = ImageDto.Description,
                    CreatedDate = DateTime.UtcNow,
                    UserId = ImageDto.UserId,
                    ImageURL = ImageDto.ImageURL // Ensuring that the url this is set
             };

           await _imageRepo.CreateAsync(ImageModel);

            return CreatedAtAction(nameof(GetById), new { Id = ImageModel.ImageId}, ImageModel.ToImageDto());

        }

        [HttpPut]
        [Route("{id:int}")]

        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateImageRequestDto updateDto)
        {
          var ImageModel =  await _imageRepo.UpdateAsync(id, updateDto);

          if(ImageModel == null)
          {
             return NotFound();
          }

         
          return Ok(ImageModel.ToImageDto());
        }
    
        [HttpDelete]
        [Route("{idi:int}")]

        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var ImageModel = await _imageRepo.DeleteAysnc(id);

            if(ImageModel == null)
            {
                return NotFound();
            }


            
            
            return NoContent();
        }
    }
}