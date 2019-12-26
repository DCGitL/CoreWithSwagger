using Adventure.Works.Dal;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreWithSwagger.Services
{
    public interface IUserService
    {
        Task<User> AsyncAuthenticate(string username, string password);
        Task<IEnumerable<User>> GetAsyncAllUsers();

    }
}
