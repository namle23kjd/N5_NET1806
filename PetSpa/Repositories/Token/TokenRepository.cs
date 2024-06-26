﻿using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PetSpa.Models.Domain;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PetSpa.Repositories.Token
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IConfiguration configuration;
        public TokenRepository(IConfiguration configuration)
        {
            this.configuration= configuration;
        }
        public string CreateJWTToken(ApplicationUser user, string role,int minute)
        {
            //Create claims
            var claims = new List<Claim>();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }


            claims.Add(new Claim(ClaimTypes.Role, role));
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(minute),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
