using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWithSwagger.SwaggerVersion
{
    public class ApiExplorerGroupPerVersionConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var controllerNameSpace = controller.ControllerType.Namespace;
            var apiVersion = controllerNameSpace?.Split('.').Last().ToLower();
            controller.ApiExplorer.GroupName = apiVersion;
        }
    }
}
