using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeDBDal
{
    public  class DbEmployees
    {
        public int id { get; set; }
        public string  FirstName { get; set; }
        public string  LastName { get; set; }
        public string Gender { get; set; }
        public double?  Salary { get; set; }
    }
}
