using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Comment
{
    public class CreateCommentDto
    {
        [Required]
        [MinLength(5, ErrorMessage = "Content must be 5 characters")]
        [MaxLength(1570, ErrorMessage = "Content cannot be over 280 characters")]

         public string Content { get; set; } =  string.Empty;

       //  public string ? UserID {get; set;}

    }
}