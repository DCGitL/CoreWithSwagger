using Adventure.Works.Dal;
using Adventure.Works.Dal.Entity;
using CoreWithSwagger.Helper;
using CoreWithSwagger.Models.Entity;
using CoreWithSwagger.Models.IdentityDbContext;
using CoreWithSwagger.Models.Refresh.Token.DBContext;
using CoreWithSwagger.Models.Refresh.Token.DBContext.Entity;
using CoreWithSwagger.Models.Request;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
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
        private readonly AppIdentityDbContext appIdentityDbContext;

        private DateTime defaultExpirationTime = DateTime.UtcNow.AddSeconds(45);
        

        public UserService(IOptions<AppSettings> _appSettings, TokenValidationParameters tokenValidationParameters, UserManager<ApplicationUser> userManager, AppIdentityDbContext appIdentityDbContext)
        {
            appSettings = _appSettings.Value;
            _tokenValidationParameters = tokenValidationParameters;
            this.userManager = userManager;
            this.appIdentityDbContext = appIdentityDbContext;
            usersDbContext = new DatabaseUsers(this.appSettings.DatabaseConnectionstring);
            defaultExpirationTime = DateTime.UtcNow.Add(appSettings.TokenLifeTime);
           
             
        }

        public async Task<UserJwtToken> AsyncAuthenticate(string username, string password)
        {
            bool isUserValid = false;
            var appUser = await userManager.FindByNameAsync(username);

            if (appUser != null && await userManager.CheckPasswordAsync(appUser, password))
            {
                isUserValid = true;
            }

            if (!isUserValid)
            {
                return null;
            }

            var jwtToken = await GenerateJwtToken(appUser);

            return jwtToken;
        }


        public async Task<IEnumerable<User>> GetAsyncAllUsers()
        {


            return await usersDbContext.GetAsyncAllUsers();
        }



        public async Task<UserJwtToken> GenerateJwtToken(ApplicationUser appUser)
        {

            var appUserRoles = await userManager.GetRolesAsync(appUser);

            //var user = await usersDbContext.GetAsyncAuthenticatedUser(username, password);
            //if (user == null)
            //    return null;
            var tokenHandler = new JwtSecurityTokenHandler();


            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, appUser.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, GenerateRefreshToken()),
                new Claim(ClaimTypes.Name, appUser.UserName),
                new Claim(JwtRegisteredClaimNames.Email, appUser.Email),
                new Claim("id", appUser.Id)

            };

            if (appUserRoles != null && appUserRoles.Count > 0)
            {
                foreach (var role in appUserRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            if (defaultExpirationTime < DateTime.UtcNow)
            {
                TimeSpan duration = appSettings.TokenLifeTime;
                defaultExpirationTime = DateTime.UtcNow.Add(duration);
            }

           
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims, "Bearer"),
                NotBefore = DateTime.UtcNow.AddMinutes(-1),
                IssuedAt = DateTime.UtcNow,
                Expires = defaultExpirationTime,
                Audience = "http://my.audience.com",
                Issuer = "http://tokenissuer.com",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            UserJwtToken userToken = new UserJwtToken();
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            userToken.AccessToken = tokenHandler.WriteToken(token);

            userToken.AccessTokenExpiration = defaultExpirationTime;
            var tokenid = token.Id;
            var _refreshToken = new JwtRefreshToken
            {
               
                JwtId = token.Id,
                UserId = appUser.Id,
                CreatedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)

            };

            try
            {
                await appIdentityDbContext.JwtRefreshTokens.AddAsync(_refreshToken);
                await appIdentityDbContext.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                var msg = ex.Message;

                throw ex;
            }


            userToken.RefreshToken = _refreshToken.RefreshToken;
            return userToken;
        }

        /// <summary>
        /// Generates a new jwt token and a refresh token
        /// </summary>
        /// <param name="token">Jwt token to be refreshed</param>
        /// <param name="refreshToken">Refreshtoken that is used to refresh the jwt token</param>
        /// <returns></returns>
        public async Task<RefreshTokenResponse> RefreshTokenAsyc(string token, string refreshToken)
        {
            RefreshTokenResponse refreshTokenResult = null;

            var validatedToken = GetPrincipalFromToken(token);
            if (validatedToken == null)
            {
                //token is invalid
                return new RefreshTokenResponse { Errors = new[] { "Invalid Token " } };
            }
            _tokenValidationParameters.RequireExpirationTime = true;

            var exptime = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value;

            var expiryDateUnix = long.Parse(exptime);

            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                                       .AddSeconds(expiryDateUnix);


            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                return new RefreshTokenResponse { Errors = new[] { "This token is not yet expired" } };
            }

            //get id from the jwt token
            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = await appIdentityDbContext.JwtRefreshTokens.SingleOrDefaultAsync(r => r.RefreshToken == refreshToken);

            if (storedRefreshToken == null)
            {
                return new RefreshTokenResponse { Errors = new[] { "This refresh token does not exist" } };
            }

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                return new RefreshTokenResponse { Errors = new[] { "This refresh token has expired" } };
            }

            if (storedRefreshToken.Invalidated)
            {
                return new RefreshTokenResponse { Errors = new[] { "This refresh token has been invalidated" } };
            }

            if (storedRefreshToken.Used)
            {
                return new RefreshTokenResponse { Errors = new[] { "This refresh token has been used" } };
            }

            if (storedRefreshToken.JwtId != jti)
            {
                return new RefreshTokenResponse { Errors = new[] { "This refresh token does not match this JWT" } };
            }

            storedRefreshToken.Used = true;
            appIdentityDbContext.JwtRefreshTokens.Update(storedRefreshToken);
            await appIdentityDbContext.SaveChangesAsync();

            //get the user id from the token
            var userId = validatedToken.Claims.Single(x => x.Type == "id").Value;
            var user = await userManager.FindByIdAsync(userId);

           
            var generateRefreshToken = await GenerateJwtToken(user);

            refreshTokenResult = new RefreshTokenResponse
            {
                Token = generateRefreshToken.AccessToken,
                RefreshToken = generateRefreshToken.RefreshToken,
                Success = true,
                AccessTokenExpiration = generateRefreshToken.AccessTokenExpiration
            };

            return refreshTokenResult;

        }

        private string GenerateRefreshToken()
        {
            var randomnumber = new byte[32];
            string result = string.Empty;

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomnumber);

                return Convert.ToBase64String(randomnumber);
            }
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                _tokenValidationParameters.ValidateLifetime = false;
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
                if (!IsJWtWithValidSecurityAlgorithm(validatedToken))
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
