using System;
using System.Collections.Generic;
using System.Text;

namespace Dal
{
    public class Employee
    {
        public int EmpoyeeKey { get; set; }

        public string FirstName  { get; set; }
        public string LastName  { get; set; }
        public string  Title { get; set; }
        public DateTime? HireDate { get; set; }
    }
}
