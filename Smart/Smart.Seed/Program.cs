using System;
using System.Collections.Generic;
using Smart.Business;
using Smart.Objects;
using Nest;
using Smart.Data.Model;
using System.IO;
using Newtonsoft.Json;
using Smart.Business.Interface;
using Smart.Business.Implementation;
using Smart.Objects.Model;
using System.Linq;
using System.Text.RegularExpressions;

namespace Autocomplete.Feed
{
    class Program
    {
        private static readonly Regex sWhitespace = new Regex(@"\s+");
        public static string ReplaceWhitespace(string input)
        {
            return sWhitespace.Replace(input, "");
        }
        static void Main(string[] args)
        {
            List<PropertyModel> properties = new List<PropertyModel>();
            List<ManagementModel> managementCompanies = new List<ManagementModel>();

            var connectionSettings = new ConnectionSettings(new Uri("https://search-apartment-5hhvuqjxeuwos2vgxnocvcw5vi.us-east-2.es.amazonaws.com/"))
                                    .BasicAuthentication("admin", "Lkjhgf#$321");

            #region Seed Apartment Properties
            using (StreamReader r = File.OpenText("PropertieJson.txt"))
            {
                string file = r.ReadToEnd();
                properties = JsonConvert.DeserializeObject<List<PropertyModel>>(file);
                properties.ForEach(s => s.property.market = ReplaceWhitespace(s.property.market));
                properties.ForEach(s => s.property.state = ReplaceWhitespace(s.property.state));
            }

            IPropertyService propertiesService = new PropertyService(connectionSettings);

            string productSuggestIndex = "apartmentproperty";

            bool isCreated = propertiesService.CreateIndexAsync(productSuggestIndex).Result;

            if (isCreated)
            {
                propertiesService.AddManyAsync(productSuggestIndex, properties).Wait();
            }
            #endregion

            #region Seed Management Companies
            using (StreamReader r = File.OpenText("ManagementJson.txt"))
            {
                string file = r.ReadToEnd();
                managementCompanies = JsonConvert.DeserializeObject<List<ManagementModel>>(file);
                properties.ForEach(s => s.property.market = ReplaceWhitespace(s.property.market));
                properties.ForEach(s => s.property.state = ReplaceWhitespace(s.property.state));
            }

            IManagementService mgmtService = new ManagementService(connectionSettings);

            string mgmtSuggestIndex = "managementcompany";

            bool isMgmtCreated = mgmtService.CreateIndexAsync(mgmtSuggestIndex).Result;

            if (isMgmtCreated)
            {
                mgmtService.AddManyAsync(mgmtSuggestIndex, managementCompanies).Wait();
            }
            #endregion

            #region Seed Market Region

            var allMarkets = properties.Select(s => new MarketRegion()
            {
                market = s.property.market,
                state = s.property.state
            }).ToList();

            allMarkets.AddRange(managementCompanies.Select(s => new MarketRegion()
            {
                market = s.mgmt.market,
                state = s.mgmt.state
            }).ToList());

            var distinctMarket = allMarkets.GroupBy(g => new { g.market, g.state }).Select(s => new MarketRegion()
            {
                market = s.Key.market,
                state = s.Key.state
            }).ToList();

            IMarketRegionService marketRegionService = new MarketRegionService(connectionSettings);

            string marketRegionIndex = "marketregion";

            bool isRegionCreated = marketRegionService.CreateIndexAsync(marketRegionIndex).Result;
            if (isRegionCreated)
            {
                marketRegionService.AddManyAsync(marketRegionIndex, distinctMarket).Wait();
            }
            #endregion

        }
    }
}