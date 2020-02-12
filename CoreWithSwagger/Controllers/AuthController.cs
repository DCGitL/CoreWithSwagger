using Adventure.Works.Dal;
using Adventure.Works.Dal.Entity;
using CoreWithSwagger.Models.Auth;
using CoreWithSwagger.Models.Request;
using CoreWithSwagger.Services;
using CoreWithSwagger.SwaggerFilters.Examples;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreWithSwagger.Controllers
{

    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService userService;

        public AuthController(IUserService userService)
        {
            this.userService = userService;
        }

        /// <summary>
        /// Authenticate users
        /// </summary> 
        /// <param name="userInfo"></param>
        /// <returns></returns>

        [HttpPost, Route("Login")]
        [MapToApiVersion("2.0")]
        [AllowAnonymous]
        [SwaggerResponse(200, description:"Get user Information", Type = typeof(UserJwtToken))]
        [Produces(contentType:"application/json",additionalContentTypes: new string[] {"application/xml"})]
        public async Task<ActionResult<UserJwtToken>> Login([FromBody] RequestUser userInfo )
        {

            var url = Request.GetDisplayUrl();
            var user = await this.userService.AsyncAuthenticate(userInfo.UserName, userInfo.Password);
            if(user == null)
            {
                return BadRequest(new { message = "Username or password is incorect" });
            }


            return Ok(user);

        }

        /// <summary>
        /// Get a refreshed token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        [HttpPost, Route("RefreshToken")]
        [MapToApiVersion("2.0")]
        [AllowAnonymous]
        //[SwaggerResponse(200, description: "Get user Information", Type = typeof(User))]
        [Produces(contentType: "application/json", additionalContentTypes: new string[] { "application/xml" })]
        public async Task<ActionResult<RefreshTokenResponse>> RefreshToken([FromBody] RefreshTokenRequest refreshToken)
        {

            var url = Request.GetDisplayUrl();
            var tokenRefresh = await this.userService.RefreshTokenAsyc(refreshToken.Token, refreshToken.RefreshToken);
            if (tokenRefresh == null)
            {
                return BadRequest(new { message = "Unable to create refreshed token" });
            }
            
            return Ok(tokenRefresh);

        }


        /// <summary>
        /// Get all the users that have access to this api
        /// </summary>
        /// <returns>An Array of Users with roles</returns>
        /// <remarks>
        /// Note to access this endpoint this user must have admin rights
        /// </remarks>
        [HttpGet, Route("AllApiUsers")]
        [Authorize(Roles ="Admin")]
        [MapToApiVersion("2.0")]
        [Produces(contentType: "application/json", additionalContentTypes: new string[] { "application/xml" })]
        public async Task<ActionResult<IEnumerable<User>> > GetAll()
        {
            var username = User.Identity.Name;
            var isroleAdmin =  User.IsInRole("Admin");
            var users = await this.userService.GetAsyncAllUsers();

            return Ok(users);
        }

        
    }
 


}