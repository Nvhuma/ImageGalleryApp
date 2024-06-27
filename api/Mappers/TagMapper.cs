using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Tag;
using api.Models;

namespace api.Mappers
{
    public  static class TagMapper
    {
        public static TagDto ToTagDto(this Tag tagModel)
        {
            return new TagDto
            {
                 TagId = tagModel.TagId,
                 TagName = tagModel.TagName,
            };
        }
        
        public static Tag ToTagFromCreateDTO(this CreateTagRequestDto tagDto)
        {
            return new Tag
            {
                TagName = tagDto.TagName
            };
        }
    }
}