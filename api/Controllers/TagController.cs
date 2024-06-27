using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Tag;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/Tag")]
    [ApiController]

    public class TagController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly ITagRepository _tagRepo;
        public TagController(ApplicationDBContext context ,ITagRepository tagRepo)
        {
            _tagRepo = tagRepo;
           _context = context;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tag = await _tagRepo.GetAllAsync();

           var  tagDto = tag.Select(s => s.ToTagDto());

            return Ok(tag);
        }

        [HttpGet("{id}")]
        public async  Task<IActionResult> GetById([FromRoute] int id)
        {
            var tag = await _tagRepo.GetByIdAysnc(id);

            if(tag == null)
            {
                return NotFound();
            }

            return Ok(tag.ToTagDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTagRequestDto TagDto)
        
        {
                 var tagModel = TagDto.ToTagFromCreateDTO();

                 await _tagRepo.CreateAysnc(tagModel);

                 return CreatedAtAction(nameof(GetById), new {id = tagModel.TagId}, tagModel.ToTagDto());
        }
        
        [HttpPut]
        [Route("{id}")]

        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateTagRequestDto updateDto)
        {
             var tagModel =  await _tagRepo.UpdateAysnc(id, updateDto);

             if(tagModel == null)
             {
                return NotFound();
             }

           

             return Ok(tagModel.ToTagDto());
        }

        [HttpDelete]
        [Route("{id}")]

        public  async Task<IActionResult> Delete([FromRoute] int id)
        {
            var tagModel =  await _tagRepo.DeleteAysnc(id);

            if (tagModel == null)
            {
                return NotFound();
            }

           

            return NoContent();
        }

    }
}