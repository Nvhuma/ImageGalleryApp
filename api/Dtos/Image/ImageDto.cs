using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.ImageDto.Comments;
using api.Dtos.Tag;

namespace api.Dtos.Image
{
    public class ImageDto
    {
         public int ImageId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        public string ImageURL {get; set;}
        public DateTime CreatedDate { get; set; }
        public string UserId { get; set; }

        public string AppUser { get; set;}

        public List<CommentDto> Comments {get; set;} 
        public List<TagDto> Tags {get; set;}
     
    }
}