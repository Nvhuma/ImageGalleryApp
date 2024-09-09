using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interfaces
{
    // Interface defining operations for managing Comment ent
    public interface ICommentRepository
    {
        // Retrieves all comments asynchronously
        Task<List<Comment>> GetAllAsync();

        // Retrieves a specific comment by its ID asynchronously
        // Returns null if the comment is not found
        Task<Comment?> GetByIdAsync(int id);

        // Creates a new comment asynchronously
        Task<Comment> CreateAysnc(Comment commentModel);

        // Deletes a comment by its ID asynchronously
        // Returns the deleted comment or null if not found
        Task<Comment?> DeleteAsync(int id);

        // Checks if an image with the given ID exists
        Task<bool> ImageExist(int id);

        // Updates an existing comment asynchronously
        // Returns the updated comment or null if not found
        Task<Comment?> UpdateAsync(int id, Comment commentModel);
    }
}