using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Image;
using api.Models;

namespace api.Mappers
{
    public static class ImageMappers
    {
        public static ImageDto ToImageDto(this Image ImageModel)
        {
            return new ImageDto
            {
                ImageId = ImageModel.ImageId,
              UserId = ImageModel.UserId,
                Title = ImageModel.Title,
                Description = ImageModel.Description,
                CreatedDate = ImageModel.CreatedDate,
                ImageURL = ImageModel.ImageURL,
                 //AppUser = ImageModel. AppUser,


                Comments = ImageModel.Comments.Select(c => c.ToCommentDto()).ToList()

            };
        }

        public static Image ToImageFromCreateDTO(this CreateImageRequestDto imageDto)
        {
            return new Image
            {
              // UserId = imageDto.UserId,
                Title = imageDto.Title,
                Description = imageDto.Description,
                CreatedDate = imageDto.CreatedDate,
                ImageURL = imageDto.ImageURL
            };
        }
    }
}