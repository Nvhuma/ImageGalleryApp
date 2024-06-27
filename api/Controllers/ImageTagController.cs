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



        [HttpGet("{id:int}")]

        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            if(!ModelState.IsValid)
            return BadRequest(ModelState);

            var  imageTags =  await _imageTagRepo.GetByIdAsync(id);

            if(imageTags == null)
            {
                return NotFound();
            }
            return Ok(imageTags.ToImageTagDto());
        }

        [HttpPost]
        [Route("{id:int}")]
        public async Task<IActionResult> Create([FromBody] CreateImageTagRequestDto imageTagDto)
        {
             if(!ModelState.IsValid)
            return BadRequest(ModelState);

            var imageTagModel = imageTagDto.ToImageTagFromCreateDTO();
           await _imageTagRepo.CreateAsync(imageTagModel);

            return CreatedAtAction(nameof(GetById), new { id = imageTagModel.ImageId }, imageTagModel.ToImageTagDto());
        }
        

        [HttpPut]
        [Route("{id:int}")]
        public async Task< IActionResult>  Update([FromRoute] int id, [FromBody] UpdateImageTagRequestDto updateDto)
        {
            if(!ModelState.IsValid)
            return BadRequest(ModelState);

            var imageTagModel = await  _imageTagRepo.UpdateAsync(id, updateDto);

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