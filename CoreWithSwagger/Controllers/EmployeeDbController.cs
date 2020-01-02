using CoreWithSwagger.Cache;
using EmployeeDBDal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet , Route("GetAllDbEmployees")]
        [MapToApiVersion("2.0")]
        [Cached(600)]
        public async Task<IActionResult> GetEmployees()
        {
            var result = await repository.GetDbAsyncEmployees();

            return Ok(result);
        }

        [HttpGet, Route("GetEployeeById/{id:int}") ]
        [MapToApiVersion("1.0")]
        [Cached(600)]
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