using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
     [Table("ImageTags")]
    public class ImageTag
    {
         public int ImageTagID { get; set; }
        public int ImageId { get; set; }
        public int TagId { get; set; }

        // Navigation properties
        public Image Image { get; set; } // The image associated with the tag
        public Tag Tag { get; set; }  // The tag associated with the image
    }
}