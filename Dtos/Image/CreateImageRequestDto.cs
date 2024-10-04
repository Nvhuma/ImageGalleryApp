using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.ImageDto.Comments;

namespace api.Dtos.Image
{
	public class CreateImageRequestDto
	{
		[Required]
		[MinLength(2, ErrorMessage = "title must be more 5 characters")]
		[MaxLength(10000000, ErrorMessage = "title cannot be over 10000000 characters")]
		public string Title { get; set; } = string.Empty;

		[Required]
		[MinLength(5, ErrorMessage = "title must be 5 characters")]
		[MaxLength(500000, ErrorMessage = "title cannot be over 10000000 characters")]
		public string Description { get; set; } = string.Empty;


		[Required]
		public string ImageURL { get; set; }
		[Required]
		public DateTime CreatedDate { get; set; }

		public IFormFile Image { get; set; } // For file upload

	}
}
