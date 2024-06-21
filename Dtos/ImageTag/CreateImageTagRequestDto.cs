using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.ImageTag
{
    public class CreateImageTagRequestDto
    {
         [Required]
          public int ImageId { get; set; }

          [Required]
          public int TagId { get; set; }
    }
}