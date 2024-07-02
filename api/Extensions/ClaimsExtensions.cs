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
        // Define a list of possible claim types for username
        var possibleClaimTypes = new[]
        {
            "http://schemas.xmlsoap.org/ws/2005/05/Identity/claims/givenname",
            "http://schemas.xmlsoap.org/ws/2005/05/Identity/claims/name",
            "http://schemas.xmlsoap.org/ws/2005/05/Identity/claims/emailaddress",
            "http://schemas.xmlsoap.org/ws/2005/05/Identity/claims/nameidentifier"
        };

        // Try to find the first claim that matches any of the possible claim types
        foreach (var claimType in possibleClaimTypes)
        {
            var claim = user.Claims.SingleOrDefault(x => x.Type.Equals(claimType));
            if (claim != null)
            {
                return claim.Value;
            }
        }

        // If no matching claim is found, return a default value or throw an exception
        return "Username not found"; // or handle it as needed
    }
}

}            
        

    


