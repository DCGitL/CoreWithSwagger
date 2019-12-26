using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Adventure.Works.Dal;
using CoreWithSwagger.Helper;
using CoreWithSwagger.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CoreWithSwagger.Services
{
    public class UserService : IUserService
    {
        private readonly AppSettings appSettings;

        private DatabaseUsers usersDbContext = null;

        public UserService(IOptions<AppSettings> _appSettings)
        {
            appSettings = _appSettings.Value;
      
            usersDbContext = new DatabaseUsers(this.appSettings.DatabaseConnectionstring);
        }

        public async Task< User> AsyncAuthenticate(string username, string password)
        {
            var user = await usersDbContext.GetAsyncAuthenticatedUser(username, password);
            if (user == null)
                return null;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
               
            };

            if(user.roles != null && user.roles.Count > 0)
            {
                foreach(var role in user.roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims, "Bearer"),
                NotBefore= DateTime.UtcNow.AddMinutes(-1),
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(7),
                Audience = "http://my.audience.com",
                Issuer = "http://tokenissuer.com",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)                
            };

            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
            user.Password = null;

            return user;
        }

        public async Task< IEnumerable<User>> GetAsyncAllUsers()
        {
           

            return await usersDbContext.GetAsyncAllUsers();
        }

         
    }
}
