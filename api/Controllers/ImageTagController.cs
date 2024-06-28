using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using api.Mappers;
using api.Dtos.ImageTag;
using api.Interfaces;
using api.Helpers;



namespace api.Controllers
{
    [Route("api/ImageTag")]
    [ApiController]
    public class ImageTagController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IImageTagRepository _imageTagRepo;
        public ImageTagController(ApplicationDBContext context, IImageTagRepository imageTagRepo)
        {
            _imageTagRepo = imageTagRepo;
             _context = context;
        }


        // GET: api/ImageTag
        [HttpGet]
        public  async Task<IActionResult> GetAll( [FromQuery]  QueryObject query )
        {
            if(!ModelState.IsValid)
            return BadRequest(ModelState);

           var imageTags = await _imageTagRepo.GetAllAsync(query);

           var ImageTagDto = imageTags.Select(it => it.ToImageTagDto());

           return Ok(imageTags.ToList());
        }



        [HttpGet]
        [Route("{ImageTagId:int}")]

        public async Task<IActionResult> GetById([FromRoute] int ImageTagID)
        {
            if(!ModelState.IsValid)
            return BadRequest(ModelState);

            var  imageTags =  await _imageTagRepo.GetByIdAsync(ImageTagID);

            if(imageTags == null)
            {
                return NotFound();
            }
            return Ok(imageTags.ToImageTagDto());
        }

        [HttpPost]
        [Route("{Id}")]
        public async Task<IActionResult> Create(int ImageTagId, [FromBody] CreateImageTagRequestDto imageTagDto)
        {
             if(!ModelState.IsValid)
            return BadRequest(ModelState);

            var imageTagModel = imageTagDto.ToImageTagFromCreateDTO();
           await _imageTagRepo.CreateAsync(imageTagModel);

            return CreatedAtAction(nameof(GetById), new { id = imageTagModel.ImageTagID }, imageTagModel.ToImageTagDto());
        }
        

        [HttpPut]
        [Route("{id:int}")]
        public async Task< IActionResult>  Update([FromRoute] int ImageTagId, [FromBody] UpdateImageTagRequestDto updateDto)
        {
            if(!ModelState.IsValid)
            return BadRequest(ModelState);

            var imageTagModel = await  _imageTagRepo.UpdateAsync(ImageTagId, updateDto);

            if (imageTagModel == null)
            {
                return NotFound();
            }


            return Ok(imageTagModel.ToImageTagDto());
        }

        [HttpDelete]
        [Route("{id:int}")]
        public  async Task<IActionResult> Delete([FromRoute] int id)
        {
            if(!ModelState.IsValid)
            return BadRequest(ModelState);

            var imageTagModel = await _imageTagRepo.DeleteAsync(id);

            if (imageTagModel == null)
            {
                return  NotFound();
            }
        

            return NoContent();
        }

    }
}