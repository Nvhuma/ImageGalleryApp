using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interfaces
{
    public interface ICommentRepository
    {
        Task<List<Comment>> GetAllAsync();
        Task<Comment?> GetByIdAsync(int id);
        Task<Comment> CreateAysnc(Comment commentModel);

        Task<Comment?> DeleteAsync(int id);

        Task<bool> ImageExist(int id);

        Task<Comment?> UpdateAsync(int id, Comment commentModel);


    }
}