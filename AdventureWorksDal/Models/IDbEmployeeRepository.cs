using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeDBDal.Models
{
    public interface IDbEmployeeRepository
    {
        Task<IEnumerable<DbEmployees>> GetDbAsyncEmployees();
        Task<DbEmployees> GetDbAsyncEmployee(int employeeid);
    }
}
