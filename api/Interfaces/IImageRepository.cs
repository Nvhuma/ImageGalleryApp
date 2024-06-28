using api.Dtos.Image;
using api.Helpers;
using api.Models;

namespace api.Interfaces
{
    public interface IImageRepository
    {
     
     Task<List<Image>> GetAllAsync(QueryObject query);
     Task<Image?> GetByIdAsync(int id);
     Task<Image> CreateAsync(Image imageModel);

     Task<Image?> UpdateAsync(int id, UpdateImageRequestDto imageDto);

     Task<Image?> DeleteAysnc(int id);

      
    }
}