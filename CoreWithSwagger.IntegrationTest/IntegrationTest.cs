using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CoreWithSwagger.Models.Auth;
using Adventure.Works.Dal.Entity;

namespace CoreWithSwagger.IntegrationTest
{
    public class IntegrationTest
    {
        protected readonly HttpClient TestClient;
        public IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder => {
                    builder.ConfigureServices(services =>
                    {
                     //  services.
                    });
                });
            TestClient = appFactory.CreateClient();
        }


        protected async Task AuthenticateAsync()
        {
            TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetJwtTokenAsync());
        }


        private async Task<string> GetJwtTokenAsync()
        {
            var authuri = "api/v2.0/auth/login";
            var response = await TestClient.PostAsJsonAsync(authuri, new RequestUser
            {
                UserName = "dac@david.com",
                Password = "1qaz@WSX"
            });

            var authresponse = await response.Content.ReadAsAsync<UserJwtToken>();
            return authresponse.AccessToken;

        }
    }
}
