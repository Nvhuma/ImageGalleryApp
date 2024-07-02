using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Image;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class ImageRepository : IImageRepository
    {
        private readonly ApplicationDBContext _context;
        public ImageRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public  async Task<Image> CreateAsync(Image imageModel)
        {
            if (string.IsNullOrEmpty(imageModel.ImageURL))
               {
                    throw new ArgumentException("ImageURL cannot be null or empty");
               }
               
            await _context.Images.AddAsync(imageModel);

            await _context.SaveChangesAsync();

            return imageModel;
        }

        public async  Task<Image?> DeleteAysnc(int id)
        {
           var imageModel = await _context.Images.FirstOrDefaultAsync(x => x.ImageId == id);

           if (imageModel == null)
           {
             return null;
           }

           _context.Images.Remove(imageModel);

           await _context.SaveChangesAsync();

           return imageModel;
        }

        // public async Task<List<Image>> GetAllAsync(QueryObject query)
        // {
        //     var images =  _context.Images.Include(  c => c.Comments)AsQueryable();

        //     if(!string.IsNullOrWhiteSpace(query.ImageId))
        //     {
        //         images = images.Where(s => s.ImageId.Contains(query.ImageId));
        //     }

        //     if(!string.IsNullOrWhiteSpace(query.))
        // }
        public async Task<List<Image>> GetAllAsync(QueryObject query)

        {
            // Create a queryable list of image tags from the database.
        var images = _context.Images.Include(c => c.Comments).ThenInclude(a => a.AppUser).AsQueryable();

           // Check if the query's ImageId is not null and not the default value (0).
        if (!IsNullOrDefault(query.ImageId))
        {
            // If ImageId is valid, filter the image tags to include only those with the specified ImageId.
            images = images.Where(it => it.ImageId == query.ImageTagID);
        }
            // Convert the filtered queryable list to a List<ImageTag> and return it asynchronously.
        return await images.ToListAsync();
        }


        private bool IsNullOrDefault(int? ImageId)
        {
             // Check if the nullable integer 'imageId' is either null or the default value (0).
             // Return true if it is null or 0, otherwise return false.
          return !ImageId.HasValue || ImageId.Value == default(int);
        }
        


        public async Task<Image?> GetByIdAsync(int id)
        {
            return await _context.Images.FindAsync(id);
        }

        public async Task<Image?> UpdateAsync(int id, UpdateImageRequestDto imageDto)
        {
           var existingImage = await _context.Images.FirstOrDefaultAsync(x => x.ImageId == id);

           if(existingImage == null)
           {
            return null;
           }

             existingImage.ImageId = imageDto.ImageId;
             existingImage.Title = imageDto.Title;
             existingImage.ImageURL = imageDto.ImageURL;

             await _context.SaveChangesAsync();

             return existingImage;


        }
    }
}