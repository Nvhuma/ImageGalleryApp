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
            var imageTagModel = await _context.ImageTags.FirstOrDefaultAsync(x => x.TagId == id);

            if(imageTagModel == null)
            {
                return null;
            }
        
            _context.ImageTags.Remove(imageTagModel);
            await _context.SaveChangesAsync();
            return imageTagModel;
        }

        public async Task<List<ImageTag>> GetAllAsync(QueryObject query)

        {
            // Create a queryable list of image tags from the database.
        var imageTags = _context.ImageTags.AsQueryable();

           // Check if the query's ImageId is not null and not the default value (0).
        if (!IsNullOrDefault(query.ImageId))
        {
            // If ImageId is valid, filter the image tags to include only those with the specified ImageId.
            imageTags = imageTags.Where(it => it.ImageId == query.ImageId);
        }
            // Convert the filtered queryable list to a List<ImageTag> and return it asynchronously.
        return await imageTags.ToListAsync();
        }

        private bool IsNullOrDefault(int? imageId)
        {
             // Check if the nullable integer 'imageId' is either null or the default value (0).
             // Return true if it is null or 0, otherwise return false.
          return !imageId.HasValue || imageId.Value == default(int);
        }


        // {
        //     var imageTags =  _context.ImageTags.Include(it => it.ImageId).AsQueryable();

        //     if(!int.IsNullOrWhiteSpace(query.ImageId))
        //     {
        //         imageTags = imageTags.Where(it => it.ImageId.Contains(query.ImageId));
        //     }

        //     if(!int.IsNullOrWhiteSpace(query.ImageId))
        //     {
        //         imageTags = imageTags.Where(it => it.ImageId.Contains(query));
        //     }

        //     return await imageTags.ToListAsync();
        // }







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

