using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace EmployeeDBDal
{
    public class EmployeeDataAccess
    {
        private readonly string connectionstring;

        public EmployeeDataAccess(string connectionstring)
        {
            this.connectionstring = connectionstring;
        }


        public async Task<IEnumerable<DbEmployees>> GetDbAsyncEmployees()
        {
            IList<DbEmployees> list = new List<DbEmployees>();
            DbEmployees emp = null;
            using (SqlConnection conn = new SqlConnection(this.connectionstring))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetEmployees";

                    if (cmd.Connection.State == ConnectionState.Closed)
                    {
                        cmd.Connection.Open();
                    }

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while(reader.Read())
                        {
                            emp = new DbEmployees();
                            emp.id = Convert.ToInt32(reader["id"]);
                            emp.FirstName = reader["FirstName"].ToString();
                            emp.LastName = reader["LastName"].ToString();
                            emp.Gender = reader["Gender"].ToString();
                            emp.Salary = reader.IsDBNull(reader.GetOrdinal("Salary")) ? 0 : (double?)Convert.ToDouble(reader["Salary"]);
                            list.Add(emp);
                            
                        }
                    }


                }
            }


            return list;

        }

        public async Task<DbEmployees> GetDbAsyncEmployee(int employeeId)
        {
            DbEmployees emp = null;
            using (SqlConnection conn = new SqlConnection(this.connectionstring))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetEmployee";
                    cmd.Parameters.Add("@employeeId", SqlDbType.Int, int.MaxValue).Value = employeeId;
                    if (cmd.Connection.State == ConnectionState.Closed)
                    {
                        cmd.Connection.Open();
                    }

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            emp = new DbEmployees();
                            emp.id = Convert.ToInt32(reader["id"]);
                            emp.FirstName = reader["FirstName"].ToString();
                            emp.LastName = reader["LastName"].ToString();
                            emp.Gender = reader["Gender"].ToString();
                            emp.Salary = reader.IsDBNull(reader.GetOrdinal("Salary")) ? 0 : (double?)Convert.ToDouble(reader["Salary"]);

                        }
                    }


                }


            }

            return emp;

        }
    }
}
