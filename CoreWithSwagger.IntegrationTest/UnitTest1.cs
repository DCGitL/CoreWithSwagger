using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace CoreWithSwagger.IntegrationTest
{
    public class UnitTest1
    {
        private readonly HttpClient _client;
        public UnitTest1()
        {
            var appFactory = new WebApplicationFactory<Startup>();
            _client = appFactory.CreateClient();
        }


        [Fact]
        public async Task Test1()
        {
            var requestUri = "api/v2.0/values";
           var response =  await _client.GetAsync(requestUri);
            if(response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync();
                if(result.IsCompleted)
                {
                    var completedValue = result.Result;
                }
            }
        }
    }
}
