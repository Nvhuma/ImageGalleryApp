using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
     [Table("Tags")]
    public class Tag
    {
         public int TagId { get; set; }
        public string TagName { get; set; } = string.Empty;


        // Navigation properties
        public List<ImageTag> ImageTags { get; set; }  // Images associated with the tag
    }
}