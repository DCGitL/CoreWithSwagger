using EmployeeDBDal.Helper;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeDBDal.Models
{
    public class DBEmployeeRepository : IDbEmployeeRepository
    {
        private EmployeeDataAccess employeeDbContext = null;
        public DBEmployeeRepository(IOptions<DbConfigOptions> options)  //string connectionstring
        {
            var connectionstring = options.Value.employeeDbConnectionstring;
            employeeDbContext = new EmployeeDataAccess(connectionstring);
        }
        public Task<DbEmployees> GetDbAsyncEmployee(int employeeid)
        {
            var result = employeeDbContext.GetDbAsyncEmployee(employeeid);
            return result;
        }

        public Task<IEnumerable<DbEmployees>> GetDbAsyncEmployees()
        {
            var results = employeeDbContext.GetDbAsyncEmployees();
            return results;
        }
    }
}
