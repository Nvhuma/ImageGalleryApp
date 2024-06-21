using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Dtos.ImageDto.Comments;
using api.Interfaces;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/comment")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepo;
        public CommentController(ICommentRepository commentRepo)
        {
            _commentRepo = commentRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if(!ModelState.IsValid)
            return BadRequest(ModelState);


            var comments = await _commentRepo.GetAllAsync();

            var CommentDto = comments.Select(s => s.ToCommentDto());


            return Ok(CommentDto);
            
        }
        [HttpGet("{id:int}")]
         public  async Task<IActionResult> getbyId([FromRoute] int id) 
       {
            if(!ModelState.IsValid)
            return BadRequest(ModelState);

        var comment =  await _commentRepo.GetByIdAsync(id);

        if(comment == null)
        {
            return NotFound();
        }

        return Ok(comment.ToCommentDto());
       }
       
       [HttpPost("{ImageId:int}")]
       public async Task<IActionResult> Create([FromRoute] int ImageId,  CreateCommentDto commentDto)
       {
            if(!ModelState.IsValid)
            return BadRequest(ModelState);


             if(!await _commentRepo.ImageExist(ImageId))
             {
                return BadRequest("comment does not exist");
             }

             var commentModel = commentDto.ToCommentFromCreate(ImageId);
             await _commentRepo.CreateAysnc(commentModel);
             
             return CreatedAtAction(nameof(getbyId), new { id = commentModel.CommentId}, commentModel.ToCommentDto());
       }

       [HttpPut]
       [Route("{id:int}")]
       public async Task<IActionResult> Update([FromRoute]int id, [FromBody] UpdateCommentRequestDto updateDto)
       {    
          if(!ModelState.IsValid)
            return BadRequest(ModelState);

         
          var comment = await _commentRepo.UpdateAsync(id, updateDto. ToCommentFromUpdate());

          if(comment == null)
          {

           return NotFound("comment not found");

          }

          return Ok(comment.ToCommentDto());
       }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if(!ModelState.IsValid)
            return BadRequest(ModelState);

            var commentModel = await _commentRepo.DeleteAsync(id);

            if(commentModel == null)
            {
                return NotFound("Comments deos not exist");
            }
        
            return  Ok(commentModel);

        }

    }
}