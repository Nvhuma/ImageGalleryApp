using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Image;
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

        public async Task<List<Image>> GetAllAsync()
        {
            return await _context.Images.ToListAsync();
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