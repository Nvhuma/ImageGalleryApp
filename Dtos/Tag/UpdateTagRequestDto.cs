using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Tag
{
    public class UpdateTagRequestDto
    {
        [Required]
          public int TagId { get; set; }

        [Required]
        [MinLength(5, ErrorMessage = "Content must be 5 characters")]
        [MaxLength(1570, ErrorMessage = "Content cannot be over 280 characters")]
        public string TagName { get; set; } = string.Empty;
    }
}