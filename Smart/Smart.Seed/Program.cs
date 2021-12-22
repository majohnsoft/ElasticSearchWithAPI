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
using Microsoft.Extensions.Logging;

namespace Smart.Seeds
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
            List<PropertyObj> properties = new List<PropertyObj>();
            List<ManagementObj> managementCompanies = new List<ManagementObj>();;

            var connectionSettings = new ConnectionSettings(new Uri("https://search-apartment-5hhvuqjxeuwos2vgxnocvcw5vi.us-east-2.es.amazonaws.com/"))
                                    .BasicAuthentication("admin", "Lkjhgf#$321");

            #region Seed Apartment Properties
            using (StreamReader r = File.OpenText("PropertieJson.txt"))
            {
                string file = r.ReadToEnd();
                var import = JsonConvert.DeserializeObject<List<ApartmentBuildingDTO>>(file);
                properties = import.Select(s => new PropertyObj(s.property)).ToList();
                properties.ForEach(s => s.market = ReplaceWhitespace(s.market));
                properties.ForEach(s => s.state = ReplaceWhitespace(s.state));
            }

            IPropertyServices propertiesService = new PropertyServices(connectionSettings);

            string apartmentSuggestIndex = Constants.APARTMENTINDEXNAME;

            bool isCreated = propertiesService.CreateIndexAsync(apartmentSuggestIndex).Result;

            if (isCreated)
            {
                propertiesService.AddManyAsync(apartmentSuggestIndex, properties).Wait();
            }
            #endregion

            #region Seed Management Companies
            using (StreamReader r = File.OpenText("ManagementJson.txt"))
            {
                string file = r.ReadToEnd();
                var import = JsonConvert.DeserializeObject<List<ManagementDTO>>(file);
                managementCompanies = import.Select(s=> new ManagementObj(s.mgmt)).ToList();
                managementCompanies.ForEach(s => s.market = ReplaceWhitespace(s.market));
                managementCompanies.ForEach(s => s.state = ReplaceWhitespace(s.state));
            }

            IManagementServices mgmtService = new ManagementServices(connectionSettings);

            string mgmtSuggestIndex = Constants.MANAGEMENTCOMPANYINDEXNAME;

            bool isMgmtCreated = mgmtService.CreateIndexAsync(mgmtSuggestIndex).Result;

            if (isMgmtCreated)
            {
                mgmtService.AddManyAsync(mgmtSuggestIndex, managementCompanies).Wait();
            }
            #endregion

            #region Seed Market Region

            var allMarkets = properties.Select(s => new MarketRegion()
            {
                market = s.market,
                state = s.state
            }).ToList();

            allMarkets.AddRange(managementCompanies.Select(s => new MarketRegion()
            {
                market = s.market,
                state = s.state
            }).ToList());

            var distinctMarket = allMarkets.GroupBy(g => new { g.market, g.state }).Select(s => new MarketRegion()
            {
                market = s.Key.market,
                state = s.Key.state
            }).ToList();

            IMarketRegionService marketRegionService = new MarketRegionService(connectionSettings);

            string marketRegionIndex = Constants.MARKETINDEXNAME;

            bool isRegionCreated = marketRegionService.CreateIndexAsync(marketRegionIndex).Result;
            if (isRegionCreated)
            {
                marketRegionService.AddManyAsync(marketRegionIndex, distinctMarket).Wait();
            }
            #endregion

        }
    }
}