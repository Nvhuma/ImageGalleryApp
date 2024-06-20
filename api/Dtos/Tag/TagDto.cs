using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Tag
{
    public class TagDto
    {
         public int TagId { get; set; }
        public string TagName { get; set; } = string.Empty;
    }
}