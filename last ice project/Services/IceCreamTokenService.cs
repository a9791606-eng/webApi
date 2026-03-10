using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace IceCreamNamespace.Services
{
    // Simple token helper for development. Replace secret with secure configuration in production.
    public static class IceCreamTokenService
    {
        private static readonly string SecretKey = "super_secret_key_123!";

        public static SecurityToken GetToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "local",
                audience: "local",
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return token;
        }

        public static string WriteToken(SecurityToken token)
        {
            return new JwtSecurityTokenHandler().WriteToken((JwtSecurityToken)token);
        }
    }
}
