using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.ImageTag;
using api.Helpers;
using api.Models;

namespace api.Interfaces
{
    public interface IImageTagRepository
    {
        Task<List<ImageTag>> GetAllAsync(QueryObject query);
        Task<ImageTag?> GetByIdAsync(int ImageTagID);
        Task<ImageTag> CreateAsync(ImageTag imageTagRepository);
        Task<ImageTag?> UpdateAsync(int id, UpdateImageTagRequestDto ImageTagDto);
        Task<ImageTag?> DeleteAsync(int id);
    }
}