using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeDBDal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreWithSwagger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class EmployeeDbController : ControllerBase
    {
        private readonly IDbEmployeeRepository repository;

        public EmployeeDbController(IDbEmployeeRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet , Route("GetAllDbEmployees")]
        public async Task<IActionResult> GetEmployees()
        {
            var result = await repository.GetDbAsyncEmployees();

            return Ok(result);
        }

        [HttpGet, Route("GetEployeeById/{id:int}") ]
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