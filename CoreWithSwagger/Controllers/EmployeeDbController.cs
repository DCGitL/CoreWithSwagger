using CoreWithSwagger.Cache;
using CoreWithSwagger.SwaggerFilters.Examples.Response;
using EmployeeDBDal;
using EmployeeDBDal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace CoreWithSwagger.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Authorize]
    public class EmployeeDbController : ControllerBase
    {
        private readonly IDbEmployeeRepository repository;

        public EmployeeDbController(IDbEmployeeRepository repository)
        {
            this.repository = repository;
        }
        /// <summary>
        /// Get All Employees
        /// </summary>
        /// <returns></returns>
        [HttpGet , Route("GetAllDbEmployees")]
        [MapToApiVersion("2.0")]
        [Cached(600)]
        [Produces("application/json")]
        [SwaggerResponse(200, description:"Get all employees",Type =typeof(IEnumerable<DbEmployees>))]
        [SwaggerResponseExample(200, typeof(DbEmployeesExamples))]
        public async Task<IActionResult> GetEmployees()
        {
            var result = await repository.GetDbAsyncEmployees();

            return Ok(result);
        }

        /// <summary>
        /// Gets a Employee base on a valid employee ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("GetEployeeById/{id:int}") ]
        [MapToApiVersion("1.0")]
        [Cached(600)]
        [SwaggerResponse(200, description: "Get employee", Type = typeof(DbEmployees),Description ="Get a specific DBEmployee")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var result = await repository.GetDbAsyncEmployee(id);
            if(result == null)
            {
                return NotFound($"No Employee found with employee id {id}");
            }
            return Ok(result);
        }
    }
}