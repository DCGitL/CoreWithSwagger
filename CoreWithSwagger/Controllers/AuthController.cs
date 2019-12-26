using Adventure.Works.Dal;
using CoreWithSwagger.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CoreWithSwagger.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService userService;

        public AuthController(IUserService userService)
        {
            this.userService = userService;
        }
        [HttpPost, Route("Login")]
        [AllowAnonymous]
        public async Task< IActionResult> Login([FromBody] User userInfo )
        {
            var user = this.userService.AsyncAuthenticate(userInfo.Username, userInfo.Password);
            if(user == null)
            {
                return await Task.FromResult(BadRequest(new { message = "Username or password is incorredt" }));
            }


            return await Task.FromResult(Ok(user));

        }

        [HttpGet, Route("AllUsers")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetAll()
        {
            var username = User.Identity.Name;
            var isroleAdmin =  User.IsInRole("Admin");
            var users = await this.userService.GetAsyncAllUsers();

            return Ok(users);
        }

        
    }
 


}