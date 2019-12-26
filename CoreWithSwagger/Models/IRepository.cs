using Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWithSwagger.Models
{
    public interface IRepository
    {
        IEnumerable<Employee> GetEmployee(int pageNumber);

        Task<IEnumerable<Employee>> GetAsyncEmployee(int pageNumber);
    }
}
