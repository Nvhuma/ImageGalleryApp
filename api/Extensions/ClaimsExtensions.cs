using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace api.Extensions
{
    public static class ClaimsExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            // Attempt to find the claim with the type "http://schemas.xmlsoap.org/ws/2005/05/Identity/claims/givenname"
            // This is typically where the username is stored in claims
            // The SingleOrDefault method will return null if no matching claim is found
            // We then access the Value property of the claim, which will throw a NullReferenceException if the claim doesn't exist
            return user.Claims.SingleOrDefault(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/Identity/claims/givenname")).Value;


            // {
            //     if (user == null)
            //     {
            //         throw new ArgumentNullException(nameof(user));
            //     }

            //     var claim = user.FindFirst(ClaimTypes.Name);
            //     if (claim == null)
            //     {
            //         throw new Exception("Username claim not found");
            //     }

            //     return claim.Value;
            // }
        }

    }

}
