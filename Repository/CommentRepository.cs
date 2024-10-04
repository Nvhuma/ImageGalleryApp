using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
	public class CommentRepository : ICommentRepository
	{
		private readonly ApplicationDBContext _context;
		private readonly UserManager<AppUser> _userMagnager;

		public CommentRepository(ApplicationDBContext context)
		{
			_context = context;
		}

		public async Task<Comment> CreateAysnc(Comment commentModel)
		{
			await _context.Comments.AddAsync(commentModel);
			await _context.SaveChangesAsync();
			return commentModel;
		}

		public async Task<Comment?> DeleteAsync(int id)
		{
			var commentModel = await _context.Comments.FirstOrDefaultAsync(x => x.CommentId == id);

			if (commentModel == null)
			{
				return null;
			}
			_context.Comments.Remove(commentModel);
			await _context.SaveChangesAsync();
			return commentModel;
		}

		public async Task<List<Comment>> GetAllAsync()
		{
			return await _context.Comments.Include(a => a.AppUser).ToListAsync();
		}



		public async Task<Comment?> GetByIdAsync(int id)
		{
			return await _context.Comments
 .Include(c => c.AppUser)  // Include the AppUser related to the comment
 .FirstOrDefaultAsync(c => c.CommentId == id);  // Retrieve comment by its id
		}

		public Task<bool> ImageExist(int id)
		{
			return _context.Images.AnyAsync(s => s.ImageId == id);
		}

		public async Task<Comment?> UpdateAsync(int id, Comment commentModel)
		{
			var existingComment = await _context.Comments.FindAsync(id);

			if (existingComment == null)
			{
				return null;
			}

			existingComment.Content = commentModel.Content;


			await _context.SaveChangesAsync();

			return existingComment;
		}



		public async Task<List<Comment>> GetCommentsByImageIdAsync(int imageId)
		{
			return await _context.Comments
			.Include(c => c.AppUser)
										.Where(c => c.ImageId == imageId)
										.ToListAsync();
		}
	}


}


