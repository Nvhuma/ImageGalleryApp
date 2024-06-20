using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.ImageDto.Comments;

namespace api.Dtos.Image
{
    public class CreateImageRequestDto
    {
         public int ImageId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

       // public List<CommentDto> Comments (get; set;)

        public DateTime CreatedDate { get; set; }
        public int UserId { get; set; }
    }
}