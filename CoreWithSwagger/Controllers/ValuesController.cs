using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreWithSwagger.Models;
using Dal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreWithSwagger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [AllowAnonymous]
    public class ValuesController : ControllerBase
    {
        private readonly IRepository repository;
       

        public ValuesController(IRepository repository)
        {
            this.repository = repository;
          
        }
        // GET api/values
        [HttpGet]
       [Produces("application/xml")]
      // [Authorize(Roles = "Admin")]
        public  ActionResult<IEnumerable<string>> Get()
        {
            var name = User.Identity.Name;
            var result = new string[] { "value1", "value2", name };
            return result;
         
        }

        // GET api/values/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public ActionResult<string> Get(int id)
        { 
            if(id ==0 )
            {
                throw new ArgumentException($"{id} cannot be 0");
            }
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id:int}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }


        [HttpGet, Route("Employees/{pageNumber:int}")]
        //[MapToApiVersion("V2")]
        public async Task<IActionResult> GetEmployee(int pageNumber = 1)
        {
            var result = await repository.GetAsyncEmployee(pageNumber);
            return Ok(result);
        }

        //[HttpGet, Route("DimEmployees")]
        //public IActionResult GetDimEmployees()
        //{
        //    var result = dimEmployee.GetAllDimEmployees();

        //    return Ok(result);
        //}
    }
}