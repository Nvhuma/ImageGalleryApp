using api.Dtos.Image;
using api.Helpers;
using api.Models;

namespace api.Interfaces
{
    // Interface defining operations for managing Image entities
    public interface IImageRepository
    {
        // Retrieves all images asynchronously, with optional filtering via QueryObject
        Task<List<Image>> GetAllAsync(QueryObject query);

        // Retrieves a specific image by its ID asynchronously
        // Returns null if the image is not found
        Task<Image?> GetByIdAsync(int id);

        // Creates a new image asynchronously
        Task<Image> CreateAsync(Image imageModel);

        // Updates an existing image asynchronously
        // Returns the updated image or null if not found
        Task<Image?> UpdateAsync(int id, UpdateImageRequestDto imageDto);

        // Deletes an image by its ID asynchronously
        // Returns the deleted image or null if not found
        Task<Image?> DeleteAysnc(int id);
    }
}