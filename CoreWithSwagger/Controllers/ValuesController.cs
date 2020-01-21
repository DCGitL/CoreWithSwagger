using CoreWithSwagger.Cache;
using CoreWithSwagger.Infrastructure;
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



        // GET api/values/5
        /// <summary>
        /// Get the Hex value from the Longitude and Latitude GPS location
        /// </summary>
        /// <param name="Lat">Latitute value must be greater than zero</param>
        /// <param name="Long">Longitude value must be lest than zero</param>
        /// <returns></returns>
        [HttpGet, Route("GetHexByLatLong/{Lat:LatLongConstraint}/{Long}")]
        [MapToApiVersion("1.0")]
        [Cached(600)]
        public async Task<ActionResult<string>> GetLatLong(double Lat, double Long)
        {
            
            return await Task.FromResult(Ok( $"value{Lat} : {Long}"));
        }



        /// <summary>
        /// Get Employees by pagination this method sends 100 employees per 
        /// page number
        /// </summary>
        /// <response code="200">[Return 100 employees per page]</response>
        /// <param name="pageNumber">The page number for the current page</param>
        /// <returns></returns>
        [HttpGet, Route("Employees/{pageNumber:int}")]
        [MapToApiVersion("2.0")]
        [Cached(600)]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetEmployee(int pageNumber = 1)
        {
            var result = await repository.GetAsyncEmployee(pageNumber);
            return Ok(result);
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


        //[HttpGet, Route("DimEmployees")]
        //public IActionResult GetDimEmployees()
        //{
        //    var result = dimEmployee.GetAllDimEmployees();

        //    return Ok(result);
        //}
    }
}