using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.ImageTag;
using api.Dtos.ImageTagDto;
using api.Models;

namespace api.Mappers
{
    public static class ImageTagMappers
    {
        public static ImageTagDto ToImageTagDto(this ImageTag imageTagModel)
        {
            return new ImageTagDto
            {
              ImageTagID = imageTagModel.ImageTagID,
              ImageId = imageTagModel.ImageId,
              TagId = imageTagModel.TagId,
            };
        }

        public static ImageTag ToImageTagFromCreateDTO(this CreateImageTagRequestDto imageTagDto)
        {
            return new ImageTag
            {
                ImageTagID = imageTagDto.ImageTagID,
                 TagId = imageTagDto.TagId,
                 ImageId = imageTagDto.ImageId,
            };
        }
    }
}
