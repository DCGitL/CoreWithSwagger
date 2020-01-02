using CoreWithSwagger.Cache;
using CoreWithSwagger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreWithSwagger.Controllers
{

    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
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
     //  [Produces("application/xml")]
      // [Authorize(Roles = "Admin")]
      [MapToApiVersion("2.0")]
      [Cached(600)]
        public  ActionResult<IEnumerable<string>> Get()
        {
            var name = User.Identity.Name;
            var result = new string[] { "value1", "value2", name };
            return result;
         
        }

        // GET api/values/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        [MapToApiVersion("1.0")]
        [Cached(600)]
        public ActionResult<string> Get(int id)
        { 
            if(id ==0 )
            {
                throw new ArgumentException($"{id} not valid");
            }
            return $"value {id}";
        }

        // POST api/values
        [HttpPost]
        [MapToApiVersion("1.0")]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id:int}")]
        [MapToApiVersion("1.0")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        [MapToApiVersion("1.0")]
        public void Delete(int id)
        {
        }


        [HttpGet, Route("Employees/{pageNumber:int}")]
        [MapToApiVersion("2.0")]
        [Cached(600)]
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