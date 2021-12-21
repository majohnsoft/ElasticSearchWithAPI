using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart.UnitTest
{
    public class MockResponses
    {
        public ConnectionSettings GetConnection()
        {
            var esconnectionSettings = new ConnectionSettings(new Uri("https://search-apartment-5hhvuqjxeuwos2vgxnocvcw5vi.us-east-2.es.amazonaws.com/"))
                                    .BasicAuthentication("admin", "Lkjhgf#$321");
            return esconnectionSettings;
        }
        public string GetMockManagementCompantJson()
        {
            var jsonString = "[{\"mgmt\": {\"mgmtID\": 27918,\"name\": \"Essex Property Trust AKA Essex Apartment Homes\"," +
                "\"market\": \"san francisco\",\"state\": \"CA\"}},{\"mgmt\": { \"mgmtID\": 24736,\"name\": \"Privately Owned and Managed\"," +
                "\"market\": \"san francisco\",\"state\": \"CA\"}}]";
            return jsonString;
        }
    }
}
