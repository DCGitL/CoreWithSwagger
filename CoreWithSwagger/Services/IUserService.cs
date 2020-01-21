using Adventure.Works.Dal;
using Adventure.Works.Dal.Entity;
using CoreWithSwagger.Models.Request;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreWithSwagger.Services
{
    public interface IUserService
    {
        Task<UserJwtToken> AsyncAuthenticate(string username, string password);
        Task<RefreshTokenRequest> RefreshTokenAsyc (string token, string refreshToken);
        Task<IEnumerable<User>> GetAsyncAllUsers();

    }
}
