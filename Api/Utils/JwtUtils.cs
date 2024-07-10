using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Api.Models;
using Microsoft.Extensions.Configuration;
namespace Api.Utils
{
    public class JwtUtils
    {
        public static string GenerateJwtToken(Usuario user, IConfiguration configuration)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])); // Obtenemos la key.
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256); // La encriptamos.

            var claims = new[]
            {
                new Claim("user",user.Nombre),
                new Claim(ClaimTypes.Role, user.Rol.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
