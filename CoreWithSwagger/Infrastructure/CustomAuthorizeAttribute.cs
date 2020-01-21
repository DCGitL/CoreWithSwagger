using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWithSwagger.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string role;

        public CustomAuthorizeAttribute(string _role) 
        {
            this.role = _role;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
           
            if(user.Identity.IsAuthenticated)
            {

            }

            if(context.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues values))
            {
                var val = values[0];
            }
        }
    }
}
