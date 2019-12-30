using Dal;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreWithSwagger.Models
{
    public class Repository : IRepository
    {
        private DataAccess dataContext;
        public Repository(string connectionString)
        {
            dataContext = new DataAccess(connectionString);
        }

        public async Task<IEnumerable<Employee>> GetAsyncEmployee(int pageNumber)
        {
            var results = await  dataContext.GetAsyncEmployees(100, pageNumber);
            return results;
        }

        public IEnumerable<Employee> GetEmployee(int pageNumber)
        {
           return  dataContext.GetEmployees(50, pageNumber);
        }
    }
}
