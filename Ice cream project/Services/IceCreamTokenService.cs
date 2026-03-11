using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using IceCreamProject.Hubs;
using IceCreamProject.Interfaces;
using IceCreamProject.Models;

namespace IceCreamProject.Services
{
    public static class IceCreamTokenService
    {
        private static readonly SymmetricSecurityKey key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("B7nLp3ZqVr6Ty0WxCs2DfGh8JkLm4NpQrSt9UvXy"));
        private static readonly string issuer = "https://icecream-demo.com";
        private static readonly string audience = "https://icecream-demo.com"; // כדאי לשנות אם יש צורך

        public static JwtSecurityToken GetToken(List<Claim> claims) =>
            new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.Now.AddDays(30.0),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

        public static TokenValidationParameters GetTokenValidationParameters() =>
            new TokenValidationParameters
            {
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = key,
                ClockSkew = TimeSpan.Zero // הסרת עיכוב כאשר הטוקן פג
            };

        public static string WriteToken(JwtSecurityToken token) =>
            new JwtSecurityTokenHandler().WriteToken(token);
    }
}
