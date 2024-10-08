using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace api.Models
{
    public class UserPasswordHistory
    {
        public int Id {get;set;}

       public string AppUserID {get; set;}  // Store the User ID as a foreign key

       public string PasswordHash {get; set;}

       public DateTime CreateDate {get; set;} = DateTime.UtcNow;

       // navigation  properties

       public AppUser AppUser { get; set;}

    }
}