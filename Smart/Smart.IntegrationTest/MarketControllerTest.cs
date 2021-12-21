using FluentAssertions;
using Smart.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Smart.IntegrationTest
{
    public class MarketControllerTest : IClassFixture<TestClientProvider<Startup>>
    {
        private HttpClient Client;

        public MarketControllerTest(TestClientProvider<Startup> fixture)
        {
            Client = fixture.Client;
        }
        [Fact]
        public async Task GetDistinctMarket()
        {
            // Arrange
            var request = new
            {
                Url = "api/market"
            };

            // Act
            var response = await Client.GetAsync(request.Url);
            var value = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

    }
}
