using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.ImageTag;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{

    public class ImageTagRepository : IImageTagRepository

    {
         private readonly ApplicationDBContext _context;
        public ImageTagRepository(ApplicationDBContext context)
        {
            _context = context;

        }

        public async Task<ImageTag> CreateAsync(ImageTag imageTagModel)
        {
           await _context.ImageTags.AddAsync(imageTagModel);
           await _context.SaveChangesAsync();
           return imageTagModel;
        }

        public async Task<ImageTag?> DeleteAsync(int id)
        {
            var imageTagModel = await _context.ImageTags.FirstOrDefaultAsync(x => x.TagId == id);

            if(imageTagModel == null)
            {
                return null;
            }
        
            _context.ImageTags.Remove(imageTagModel);
            await _context.SaveChangesAsync();
            return imageTagModel;
        }

        public  async Task<List<ImageTag>> GetAllAsync()
        {
              return await _context.ImageTags.ToListAsync();

        }

        public async Task<ImageTag?> GetByIdAsync(int id)
        {
            return  await _context.ImageTags.FindAsync(id);
        }
            

        public async Task<ImageTag?> UpdateAsync(int id, UpdateImageTagRequestDto ImageTagDto)
        {
            var existingImageTag = await _context.ImageTags.FirstOrDefaultAsync(x => x.TagId == id);

            if(existingImageTag == null)
            {
                return null;
            }
               existingImageTag.ImageId =  ImageTagDto.ImageId;
            existingImageTag.TagId =  ImageTagDto.TagId;
            
            await _context.SaveChangesAsync();

            return existingImageTag;

        }
    }
}