using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.ImageDto.Comments
{
    public class CommentDto
    {
        
        public int CommentId { get; set; }
        public string Content { get; set; } =  string.Empty;
        public int? ImageId { get; set; }
        public DateTime CreatedDate { get; set; } =   DateTime.Now;
    }
}