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

		public async Task<Image> CreateAsync(Image imageModel)
		{
			if (string.IsNullOrEmpty(imageModel.ImageURL))
			{
				throw new ArgumentException("ImageURL cannot be null or empty");
			}

			await _context.Images.AddAsync(imageModel);

			await _context.SaveChangesAsync();

			return imageModel;
		}

		public async Task<Image?> DeleteAysnc(int id)
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






		public async Task<List<Image>> GetAllAsync(QueryObject query)
		{

			IQueryable<Image> imagesQuery = _context.Images
					.Include(image => image.Comments)
							.ThenInclude(comment => comment.AppUser);


			if (!IsNullOrDefault(query.ImageId))
			{
				imagesQuery = imagesQuery.Where(image => image.ImageId == query.ImageId);
			}


			var images = await imagesQuery
					.Select(image => new Image
					{
						ImageId = image.ImageId,
						Title = image.Title,
						Description = image.Description,
						ImageURL = image.ImageURL, // Include ImageURL in the projection
						CreatedDate = image.CreatedDate,
						UserId = image.UserId, // Include other properties as needed
						Comments = image.Comments
					})
					.ToListAsync();

			return images;
		}

		public async Task<List<Image>> GetAllByUserIdAsync(QueryObject query, string UserId)
		{

			IQueryable<Image> imagesQuery = _context.Images
					.Include(image => image.Comments)
							.ThenInclude(comment => comment.AppUser);

			// Filters by UserId to only include images associated with the current user
			if (!string.IsNullOrEmpty(UserId))
			{
				imagesQuery = imagesQuery.Where(image => image.UserId == UserId);
			}

			// Checks if the query's ImageId is valid and apply filtering if necessary
			if (!IsNullOrDefault(query.ImageId))
			{
				imagesQuery = imagesQuery.Where(image => image.ImageId == query.ImageId);
			}


			var images = await imagesQuery
					.Select(image => new Image
					{
						ImageId = image.ImageId,
						Title = image.Title,
						Description = image.Description,
						ImageURL = image.ImageURL, // Include ImageURL in the projection
						CreatedDate = image.CreatedDate,
						UserId = image.UserId // Include other properties as needed
					})
					.ToListAsync();

			return images;


		}

		public async Task<Image?> GetByIdAsync(int id)
		{
			return await _context.Images.FindAsync(id);
		}

		public async Task<Image> GetImageWithCommentsAsync(int imageId)
		{
			return await _context.Images
	 .Include(i => i.Comments)
	 .FirstOrDefaultAsync(i => i.ImageId == imageId);

		}

		public async Task<Image?> UpdateAsync(int id, UpdateImageRequestDto imageDto)
		{
			var existingImage = await _context.Images.FirstOrDefaultAsync(x => x.ImageId == id);

			if (existingImage == null)
			{
				return null;
			}

			existingImage.ImageId = imageDto.ImageId;
			existingImage.Title = imageDto.Title;
			existingImage.Description = imageDto.Description;
			existingImage.ImageURL = imageDto.ImageURL;

			await _context.SaveChangesAsync();

			return existingImage;
		}

		private bool IsNullOrDefault(int? ImageId)
		{
			return !ImageId.HasValue || ImageId.Value == default(int);
		}
	}
}
