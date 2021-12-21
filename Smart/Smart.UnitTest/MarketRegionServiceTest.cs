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
    public class MarketRegionServiceTest
    {
        [Fact]
        public void CreateIndex_With_No_IndexName()
        {
            // Arrange
            var esconnectionSettings = new MockResponses().GetConnection();

            string indexName = "";
            // Act
            var isCreated = new MarketRegionService(esconnectionSettings).CreateIndexAsync(indexName).Result;
            // Assert
            Assert.False(isCreated);
        }

        [Fact]
        public void Get_All_Management_Company_Region()
        {
            // Arrange
            var esconnectionSettings = new MockResponses().GetConnection();


            // Act
            var result = new MarketRegionService(esconnectionSettings).GetAllRegionAsync(Constants.MARKETINDEXNAME).Result;
            // Assert
            Assert.True(result.Count > 0);
        }

    }
}
