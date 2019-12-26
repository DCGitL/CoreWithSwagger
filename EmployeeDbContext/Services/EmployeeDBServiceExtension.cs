using EmployeeDbContext.Models.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeDbContext.Services
{
    public static class EmployeeDBServiceExtension
    {

        public static IServiceCollection AddEmployeeDbService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<EmpDBContext>(options => options.UseSqlServer(configuration.GetConnectionString("EmployDb")));

            return services;
        }

    }
}
