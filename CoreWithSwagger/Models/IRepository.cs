using Dal;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreWithSwagger.Models
{
    public interface IRepository
    {
        IEnumerable<Employee> GetEmployee(int pageNumber);

        Task<IEnumerable<Employee>> GetAsyncEmployee(int pageNumber);
    }
}
