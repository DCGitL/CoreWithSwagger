using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CoreWithSwagger.IntegrationTest
{
    public class EmployeeDBControllerTest : IntegrationTest
    {

        [Fact]
        public async Task GetAllDbEmployees_AuthenticationAthorizatoin()
        {
            // Arrange
            await AuthenticateAsync();
            //Act
            var response = await TestClient.GetAsync("api/v2.0/EmployeeDb/GetAllDbEmployees");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

        }
    }
}
