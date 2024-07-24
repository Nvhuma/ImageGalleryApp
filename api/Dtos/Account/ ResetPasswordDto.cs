using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Account
{
    public class  ResetPasswordDto
    {
      //[Required]
    public string Token { get; set; }

     [Required]
     [EmailAddress]
     public string Email { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 15 characters long.")]
    public string NewPassword { get; set; }

    [Required]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
    }
}