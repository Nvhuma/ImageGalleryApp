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
        public int? ImageId { get; set; }
        public DateTime CreatedDate { get; set; } =   DateTime.Now;

        //nav properties
          public Image Image { get; set; } // The image the comment is associated with 
          
        //public User User { get; set; }
    }
}