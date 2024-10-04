using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.ImageTag;
using api.Helpers;
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
            var imageTagModel = await _context.ImageTags.FirstOrDefaultAsync(x => x.ImageTagID == id);

            if (imageTagModel == null)
            {
                return null;
            }

            _context.ImageTags.Remove(imageTagModel);
            await _context.SaveChangesAsync();
            return imageTagModel;
        }

        public async Task<List<ImageTag>> GetAllAsync(QueryObject query)

        {
            // Creates a queryable list of image tags from the database.
            var imageTags = _context.ImageTags.AsQueryable();

            // Checks if the query's ImageId is not null and not the default value (0).
            if (!IsNullOrDefault(query.ImageTagID))
            {
              
                imageTags = imageTags.Where(it => it.ImageId == query.ImageTagID);
            }
           
            return await imageTags.ToListAsync();
        }

        private bool IsNullOrDefault(int? ImageTagID)
        {
            
            return !ImageTagID.HasValue || ImageTagID.Value == default(int);
        }








        public async Task<ImageTag?> GetByIdAsync(int ImageTagID)
        {
            return await _context.ImageTags.FindAsync(ImageTagID);
        }


        public async Task<ImageTag?> UpdateAsync(int id, UpdateImageTagRequestDto ImageTagDto)
        {
            var existingImageTag = await _context.ImageTags.FirstOrDefaultAsync(x => x.ImageTagID == id);

            if (existingImageTag == null)
            {
                return null;
            }

            existingImageTag.ImageTagID = ImageTagDto.ImageTagID;

            existingImageTag.ImageId = ImageTagDto.ImageId;
            existingImageTag.TagId = ImageTagDto.TagId;

            await _context.SaveChangesAsync();

            return existingImageTag;

        }

        public async Task<IEnumerable<Image>> GetImagesByTagNameAsync(string tagName)
        {
             
        return await _context.Images
            .ToListAsync();
        }

       
    }
}

