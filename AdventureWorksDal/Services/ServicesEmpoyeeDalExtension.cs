using EmployeeDBDal.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeDBDal.Services
{
    public static  class ServicesEmpoyeeDalExtension
    {
        public static  IServiceCollection ServicesEmpDb(this IServiceCollection service, IConfiguration configuration)
        {

            string connection = configuration.GetConnectionString("EmployDb");
            service.AddTransient<IDbEmployeeRepository>(s => new DBEmployeeRepository(connection));

            return service;
        }
    }
}
