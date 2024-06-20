using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.ImageTag
{
    public class UpdateImageTagRequestDto
    {
          public int ImageId { get; set; }
          public int TagId { get; set; }
    }
}