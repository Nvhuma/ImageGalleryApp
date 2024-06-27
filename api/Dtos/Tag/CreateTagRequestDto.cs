using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Tag
{
    public class CreateTagRequestDto
    {
        
        // public int TagId { get; set; }
        public string TagName { get; set; } = string.Empty;
        public int ImageId { get; set; }


    }
}