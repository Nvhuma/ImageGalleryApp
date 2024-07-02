using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Dtos.ImageDto.Comments;
using api.Models;

namespace api.Mappers
{
    public static class CommentMapper
    {
        public static CommentDto ToCommentDto(this Comment commentModel)
        {
            return new CommentDto
            {
                CommentId = commentModel.CommentId,
                Content = commentModel.Content,
                CreatedDate = commentModel.CreatedDate,
                ImageId = commentModel.ImageId,
                //UserID = commentModel.UserId, 
                CreatedBy = commentModel.AppUser.UserName,

            };
        }

        public static Comment ToCommentFromCreate(this CreateCommentDto commentDto, int ImageId)
        {
            return new Comment
            {
                Content = commentDto.Content,
                ImageId = ImageId,
                //UserId = User
            };
        }

        public static Comment ToCommentFromUpdate(this UpdateCommentRequestDto commentDto)
        {
            return new Comment
            {
                Content = commentDto.Content,

            };
        }
    }
}