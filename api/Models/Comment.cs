using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    [Table("Comments")]
    public class Comment
    {
        public int CommentId { get; set; }
        public string Content { get; set; } =  string.Empty;
        public int? ImageId { get; set; } // foreing key

        public string UserId { get; set; } // Foreign key
        public DateTime CreatedDate { get; set; } =   DateTime.Now;
        
        public DateTime UploadedDate { get; set;} = DateTime.Now;

        public DateTime LastUpdated { get; set;} =  DateTime.Now;

        public string CreatedBy {get; set;} = string.Empty;

        //nav properties
       public Image Image { get; set; } // The image the comment is associated with 
          
        public AppUser AppUser { get; set; }
    }
}