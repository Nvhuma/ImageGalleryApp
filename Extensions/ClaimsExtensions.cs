using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace api.Extensions
{
public static class ClaimsExtensions
{
    public static string GetUserEmail(this ClaimsPrincipal user)
    {
        
        {
       
            
            var emailClaim = user.Claims.SingleOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
            if (emailClaim == null)
            {
                throw new InvalidOperationException("Email claim not found");
            }
            return emailClaim.Value;
        }
    }
}

}
          
        

    


