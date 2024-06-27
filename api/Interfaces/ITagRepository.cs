using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Tag;
using api.Models;

namespace api.Interfaces
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetAllAsync();
        Task<Tag?> GetByIdAysnc(int id);
        Task<Tag> CreateAysnc(Tag tagModel);
        Task<Tag?> UpdateAysnc(int id, UpdateTagRequestDto tagDto);
        Task<Tag?> DeleteAysnc(int id);
    }
}