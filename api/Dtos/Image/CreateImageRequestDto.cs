using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.ImageDto.Comments;

namespace api.Dtos.Image
{
    public class CreateImageRequestDto
    {

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

       // public List<CommentDto> Comments (get; set;)
        public string ImageURL {get; set;}
        public DateTime CreatedDate { get; set; }

         

        public string UserId { get; set; }
    }
}