using Adventure.Works.Dal;
using Adventure.Works.Dal.Entity;
using CoreWithSwagger.Helper;
using CoreWithSwagger.Models.IdentityDbContext;
using CoreWithSwagger.Models.Request;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CoreWithSwagger.Services
{
    public class UserService : IUserService
    {
        private readonly AppSettings appSettings;

        private DatabaseUsers usersDbContext = null;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly UserManager<ApplicationUser> userManager;

        public UserService(IOptions<AppSettings> _appSettings, TokenValidationParameters tokenValidationParameters, UserManager<ApplicationUser> userManager )
        {
            appSettings = _appSettings.Value;
            _tokenValidationParameters = tokenValidationParameters;
            this.userManager = userManager;
            usersDbContext = new DatabaseUsers(this.appSettings.DatabaseConnectionstring);
        }

        public async Task<UserJwtToken> AsyncAuthenticate(string username, string password)
        {
            bool isUserValid = false;
            var appUser = await userManager.FindByNameAsync(username);
           
            if (appUser != null && await userManager.CheckPasswordAsync(appUser, password))
            {
                isUserValid = true;
            }
            
            if(!isUserValid)
            {
                return null;
            }

            var appUserRoles = await userManager.GetRolesAsync(appUser);

            //var user = await usersDbContext.GetAsyncAuthenticatedUser(username, password);
            //if (user == null)
            //    return null;
            var tokenHandler = new JwtSecurityTokenHandler();
           

            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, appUser.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, appUser.UserName),
                new Claim(JwtRegisteredClaimNames.Email, appUser.Email),
                new Claim("id", appUser.Id)
               
            };

            if(appUserRoles != null && appUserRoles.Count > 0)
            {
                foreach(var role in appUserRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var expirationDate = DateTime.UtcNow.AddDays(7);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims, "Bearer"),
                NotBefore= DateTime.UtcNow.AddMinutes(-1),
                IssuedAt = DateTime.UtcNow,
                Expires = expirationDate,
                Audience = "http://my.audience.com",
                Issuer = "http://tokenissuer.com",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)                
            };

            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            UserJwtToken userToken = new UserJwtToken();
            userToken.AccessToken = tokenHandler.WriteToken(token);
            
            userToken.AccessTokenExpiration = expirationDate;

            return userToken;
        }

 
        public async Task< IEnumerable<User>> GetAsyncAllUsers()
        {
           

            return await usersDbContext.GetAsyncAllUsers();
        }

        public Task<RefreshTokenRequest> RefreshTokenAsyc(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);
            if(validatedToken == null)
            {
                return null;
            }

            var expiryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateTimeUtc = new DateTime(1970, 1,  1,  0, 0, 0, DateTimeKind.Utc)
                                       .AddSeconds(expiryDateUnix)
                                       .Subtract(appSettings.TokenLifeTime);


            if(expiryDateTimeUtc > DateTime.UtcNow)
            {

            }

            //get id from the jwt token
            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken =   usersDbContext.GetRefreshToken(token, refreshToken);

            if(storedRefreshToken == null)
            {
                return null;
            }

            if(DateTime.UtcNow > storedRefreshToken.Result.ExpiryDate)
            {
                return null;
            }

            return null;

        }


        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
           
            try
            {
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
                if(!IsJWtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }

        }


        private bool IsJWtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
