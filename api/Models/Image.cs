using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
     [Table("Images")]
    public class Image
    {
        public int ImageId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }
        public int UserId { get; set; }

        //nav properties 
         public List<Comment> Comments { get; set; } = new List<Comment>(); // Comments on the image
        public List<ImageTag> ImageTags { get; set; } = new List<ImageTag>(); // Tags associated with the image
    }
}