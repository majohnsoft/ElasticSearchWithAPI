using FluentAssertions;
using Newtonsoft.Json;
using Smart.API;
using Smart.Objects.Model;
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
    public class SearchControllerTest : IClassFixture<TestClientProvider<Startup>>
    {
        private HttpClient Client;

        public SearchControllerTest(TestClientProvider<Startup> fixture)
        {
            Client = fixture.Client;
        }
        [Fact]
        public async Task GetDistinctMarket()
        {
            // Arrange
            var market = new List<string>();
            market.Add("Austin");

            var model = new SearchRequestModel()
            {
                Keyword = "Village",
                Limit = 25,
                Market = market
            };
            var url = "api/search";
            // Act
            
            var response = await Client.PostAsync(url, ContentHelper.GetStringContent(model));
            var value = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

        }

    }
}
