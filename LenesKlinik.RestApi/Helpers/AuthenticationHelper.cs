using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LenesKlinik.Core.Entities;
using Microsoft.IdentityModel.Tokens;

namespace LenesKlinik.RestApi.Helpers
{
    public class AuthenticationHelper : IAuthenticationHelper
    {
        private byte[] secretBytes;

        public AuthenticationHelper(Byte[] secret)
        {
            secretBytes = secret;
        }

        // This method generates and returns a JWT token for a user.
        public string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim("Firstname", user.Customer.Firstname),
                new Claim("Lastname", user.Customer.Lastname),
                new Claim("UserId", user.Id.ToString()),
                new Claim("CustId", user.Customer.Id.ToString())
            };

            if (user.IsAdmin)
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));

            var token = new JwtSecurityToken(
                new JwtHeader(new SigningCredentials(
                    new SymmetricSecurityKey(secretBytes),
                    SecurityAlgorithms.HmacSha256)),
                new JwtPayload(null, // issuer - not needed (ValidateIssuer = false)
                               null, // audience - not needed (ValidateAudience = false)
                               claims.ToArray(),
                               DateTime.Now,               // notBefore
                               DateTime.Now.AddMinutes(10)));  // expires

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
