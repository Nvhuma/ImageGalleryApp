using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Tag;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class TagRepository : ITagRepository
    {
        private readonly ApplicationDBContext _context;
        public TagRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Tag> CreateAysnc(Tag tagModel)
        {
            await _context.Tag.AddAsync(tagModel);

            await _context.SaveChangesAsync();


            return tagModel;
        }

        public async Task<Tag?> DeleteAysnc(int id)
        {
            var tagModel = await _context.Tag.FirstOrDefaultAsync(x => x.TagId == id);

            if (tagModel == null)
            {
                return null;
            }

            _context.Tag.Remove(tagModel);
            await _context.SaveChangesAsync();
            return tagModel;
        }


        public async Task<List<Tag>> GetAllAsync()
        {
            return await _context.Tag.ToListAsync();
        }

        public async Task<Tag?> GetByIdAysnc(int id)
        {
            return await _context.Tag.FindAsync(id);
        }

        public async Task<IEnumerable<Image>> GetImagesByTagNameAsync(string tagName)
        {
            var images = await _context.Tag
        .Where(tag => tag.TagName == tagName)
        .SelectMany(tag => tag.ImageTags.Select(it => it.Image))
        .ToListAsync();

            return images;
        }



        public async Task<Tag?> UpdateAysnc(int id, UpdateTagRequestDto tagDto)
        {
            var existingTag = await _context.Tag.FirstOrDefaultAsync(x => x.TagId == id);

            if (existingTag == null)
            {
                return null;
            }

            
            existingTag.TagId = tagDto.TagId; // Ensuring  this field exists in both the entity and DTO
            existingTag.TagName = tagDto.TagName; // Update other fields as necessary if they arise 

            await _context.SaveChangesAsync();

            return existingTag;
        }
    }
}