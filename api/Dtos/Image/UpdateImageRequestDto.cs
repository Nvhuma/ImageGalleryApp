using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.ImageDto.Comments;

namespace api.Dtos.Image
{
    public class UpdateImageRequestDto
    {
        [Required]
         public int ImageId { get; set; }
         [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public string ImageURL {get; set;}
        public DateTime CreatedDate { get; set; }
     
    }
    
}