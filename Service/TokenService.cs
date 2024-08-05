using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using api.Interfaces;
using api.Models;
using Microsoft.IdentityModel.Tokens;

namespace api.Service
{
   
    public class TokenService : ITokenService
    {
    private readonly IConfiguration _config;
    private readonly SymmetricSecurityKey _key;

    // Constructor injecting the configuration and initializing the security key
    public TokenService(IConfiguration config)
    {
        _config = config;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]));
    }
        public string CreateToken(AppUser user)
        {
          // Define claims to be included in the token
           var claims = new List<Claim>
           {
             new Claim(JwtRegisteredClaimNames.Email, user.Email),
             new Claim(JwtRegisteredClaimNames.GivenName, user.UserName)
           };

          // Signing credentials using the symmetric key and HMAC SHA512 algorithm
           var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

           // Token descriptor to define token properties

           var tokenDescriptor = new SecurityTokenDescriptor
           {
             Subject = new ClaimsIdentity(claims),
             Expires = DateTime.Now.AddDays(7), // Token expiration time
             SigningCredentials = creds,
             Issuer = _config["JWT:Issuer"], // Token issuer
             Audience = _config["JWT:Audience"] // Token audience
           };

           var tokenHandler = new JwtSecurityTokenHandler();

            // Create and return the token

           var token = tokenHandler.CreateToken(tokenDescriptor);

           return  tokenHandler.WriteToken(token);
        }
    }
}