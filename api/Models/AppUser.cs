using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace api.Models
{
    public class AppUser : IdentityUser
    {
        public string Names { get; set;}

       // public DateTime CreatedDate {get; set;} = DateTime.Now;

        public List<Image> Images { get; set;} = new List<Image>();

        public List<Comment> Comments {get; set;} = new List<Comment>();
    }
}