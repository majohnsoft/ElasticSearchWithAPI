using Microsoft.Extensions.Logging;
using System;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smart.Business.Implementation;
using Nest;
using Xunit;
using Smart.Business.Interface;
using Elasticsearch.Net;
using Smart.Data.Model;
using Smart.Objects.Model;
using Newtonsoft.Json;
using Smart.Objects;

namespace Smart.UnitTest
{
    public class ApartmentServiceTest
    {
        [Fact]
        public void CreateIndex_With_No_IndexName()
        {
            // Arrange
            var esconnectionSettings = new MockResponses().GetConnection();

            string indexName = "";
            // Act
            var isCreated = new PropertyService(esconnectionSettings).CreateIndexAsync(indexName).Result;
            // Assert
            Assert.False(isCreated);
        }


        [Fact]
        public void Search_With_No_MarketScope()
        {
            // Arrange
            var esconnectionSettings = new MockResponses().GetConnection();
            
            var request = new SearchRequestModel()
            {
                Keyword = "Village",
                Market = null,
                IndexName = Constants.APARTMENTINDEXNAME,
                Limit = 25
            };

            // Act
            var result = new PropertyService(esconnectionSettings).SearchAsync(request).Result;
            // Assert
            Assert.True(result.Count > 0);
        }
        [Fact]
        public void Search_With_Multiple_MarketScope()
        {
            // Arrange
            var esconnectionSettings = new MockResponses().GetConnection();
            var markets = new List<string>();
           // markets.Add("savannah");
           markets.Add("austin");
           // markets.Add("houston");
            var request = new SearchRequestModel()
            {
                Keyword = "Village",
                Market = markets,
                IndexName = Constants.APARTMENTINDEXNAME,
                Limit = 25
            };

            // Act
            var result = new PropertyService(esconnectionSettings).SearchAsync(request).Result;
            // Assert
            Assert.True(result.Count > 0);
        }


        [Fact]
        public void Search_With_One_MarketScope()
        {
            // Arrange
            var esconnectionSettings = new MockResponses().GetConnection();
            var markets = new List<string>()
            {
                "savannah"
            };
            var request = new SearchRequestModel()
            {
                Keyword = "Village",
                Market = markets,
                IndexName = Constants.APARTMENTINDEXNAME,
                Limit = 25
            };

            // Act
            var result = new PropertyService(esconnectionSettings).SearchAsync(request).Result;
            // Assert
            Assert.True(result.Count > 0);
        }
    }
}
