using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Dal
{

    /// <summary>
    /// Note Dependencies for this dot net core library 
    /// System.Data.Common and System.Data.SqlClient. 
    /// Are referenced from Nuget Package
    /// </summary>
    public class DataAccess
    {
        private readonly string _connectionstring;
        public DataAccess(string connectionstring )
        {
            _connectionstring = connectionstring;
        }

        
        public IEnumerable<Employee> GetEmployees(int pageSize, int pageNumber)
        {
            IList<Employee> list = new List<Employee>();
            Employee employee = null;
            using (SqlConnection connection = new SqlConnection(_connectionstring))
            {
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "spGetScrollingEmployee";
                    cmd.Parameters.Add("@PageNumber", SqlDbType.Int, int.MaxValue).Value = pageNumber;
                    cmd.Parameters.Add("@PageSize", SqlDbType.Int, int.MaxValue).Value = pageSize;
                    if (cmd.Connection.State == ConnectionState.Closed)
                    {
                         cmd.Connection.Open();
                    }

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                       while( reader.Read())
                        {
                            employee = new Employee();
                            employee.EmpoyeeKey = Convert.ToInt32(reader["EmployeeKey"]);
                            employee.FirstName = reader["FirstName"].ToString();
                            employee.LastName = reader["LastName"].ToString();
                            employee.Title = reader.GetValue(reader.GetOrdinal("Title")).ToString();
                            employee.HireDate = (DateTime?)reader.GetValue(reader.GetOrdinal("HireDate"));

                            list.Add(employee);
                        }
                    }


                }
            }


                return list;
        }


        public async Task<IEnumerable<Employee>> GetAsyncEmployees(int pageSize, int pageNumber)
        {
            IList<Employee> list = new List<Employee>();
            Employee employee = null;
            using (SqlConnection connection = new SqlConnection(_connectionstring))
            {
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "spGetScrollingEmployee";
                    cmd.Parameters.Add("@PageNumber", SqlDbType.Int, int.MaxValue).Value = pageNumber;
                    cmd.Parameters.Add("@PageSize", SqlDbType.Int, int.MaxValue).Value = pageSize;
                    if (cmd.Connection.State == ConnectionState.Closed)
                    {
                        cmd.Connection.Open();
                    }

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            employee = new Employee();
                            employee.EmpoyeeKey = Convert.ToInt32(reader["EmployeeKey"]);
                            employee.FirstName = reader["FirstName"].ToString();
                            employee.LastName = reader["LastName"].ToString();
                            employee.Title = reader.GetValue(reader.GetOrdinal("Title")).ToString();
                            employee.HireDate = (DateTime?)reader.GetValue(reader.GetOrdinal("HireDate"));

                            list.Add(employee);
                        }
                    }


                }
            }


            return list;

        }
    }
}
