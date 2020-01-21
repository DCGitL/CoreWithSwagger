using CoreWithSwagger.Models.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Adventure.Works.Dal
{
    public class DatabaseUsers
    {
        private readonly string connectionstring;

        private IList<User> _users = new List<User>
        {
            new User { Id = "IdCode1", FirstName = "Test", LastName = "User", Username = "test", Email="test@test.com", Password = "test", roles= new List<string> {"Admin", "User", "IT" }  },
             new User { Id = "IDcode2", FirstName = "David", LastName = "Chen", Username = "Dac", Email = "david@Test.com", Password = "12345", roles= new List<string> {"Account", "User", "IT" }  }
        };
        public DatabaseUsers(string connectionstring)
        {
            if (string.IsNullOrEmpty(connectionstring))
            {
                throw new ArgumentNullException("Connection string cannot be empty");
            }
            this.connectionstring = connectionstring;
        }

        public async Task<User> GetAsyncAuthenticatedUser(string username, string password)
        {

            var user = _users.FirstOrDefault(u => u.Username == username && u.Password == password);
            user.Password = "";

            return await Task.FromResult(user);
        }


        public async Task<RefreshToken> GetRefreshToken(string token, string refreshToken)
        {
            RefreshToken _refreshToken = new RefreshToken();


            return await Task.FromResult(_refreshToken);
        }

        public async Task<IEnumerable<User>> GetAsyncAllUsers()
        {
            ///setup database code goes here
            ///
            IList<AspnetUsersRole> userroles = new List<AspnetUsersRole>();
            AspnetUsersRole userrole = null;
            using (SqlConnection conn = new SqlConnection(this.connectionstring))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "GetAllUserAndRoles";
                    if (cmd.Connection.State == System.Data.ConnectionState.Closed)
                    {
                        cmd.Connection.Open();
                    }
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while(reader.Read())
                        {
                            userrole = new AspnetUsersRole()
                            {
                                UserId = reader["id"].ToString(),
                                UserName = reader["UserName"].ToString(),
                                Email = reader["Email"].ToString(),
                              
                                RoleName = reader.IsDBNull(reader.GetOrdinal("Name")) ? string.Empty : reader["Name"].ToString()

                            };
                            userroles.Add(userrole);

                        }

                    }

                }
            }
          
            var aspusersRole = ProcessAspnetUserRoles(userroles);
            
            return aspusersRole;
         

        }




        private IEnumerable<User> ProcessAspnetUserRoles(IList<AspnetUsersRole> usersRoles)
        {
            IList<User> newUserList = new List<User>();

            User newuser = null;
            if(usersRoles != null && usersRoles.Count > 0)
            {
              var distinctUsers = usersRoles.Select(u => new 
                {
                     u.UserId,
                    u.UserName,
                    u.Email
                }).Distinct();


                foreach (var user in distinctUsers)
                {
                    newuser = new User
                    {
                        Id = user.UserId,
                        Username = user.UserName,
                        Email = user.Email,
                        roles = usersRoles.Where(u => (u.UserId == user.UserId) && !string.IsNullOrEmpty(u.RoleName)).Select(u => u.RoleName).ToList<string>()

                    };
                   
                    newUserList.Add(newuser);
                }


            }
            
            return newUserList;
        }

    }




    public class User
    {
        public string Id { get; set; }
        [Required(ErrorMessage ="First Name is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage ="Last Name is required")]
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public List<string> roles { get; set; }
    }
}
