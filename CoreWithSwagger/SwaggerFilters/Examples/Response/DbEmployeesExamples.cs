using EmployeeDBDal;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;

namespace CoreWithSwagger.SwaggerFilters.Examples.Response
{
    public class DbEmployeesExamples : IExamplesProvider<IEnumerable<DbEmployees>>
    {
        public IEnumerable<DbEmployees> GetExamples()
        {

            return new List<DbEmployees>
            {
                new DbEmployees{id = 1, FirstName="Tom", LastName ="James", Gender="Male", Salary= 87000.00},
                 new DbEmployees{id = 2, FirstName="Mary", LastName ="Lawson", Gender="Female", Salary= 87000.00},
                  new DbEmployees{id = 3, FirstName="Andrea", LastName ="Hang", Gender="Female", Salary= 87000.00}

            };
           
        }
    }
}
