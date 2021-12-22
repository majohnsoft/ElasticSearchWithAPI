using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart.Data.Model
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class PropertyObj
    {
        public int propertyID { get; set; }
        public string name { get; set; }
        public string formerName { get; set; }
        public string streetAddress { get; set; }
        public string city { get; set; }
        public string market { get; set; }
        public string state { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public CompletionField Suggest { get; set; }
        public PropertyObj(PropertyObj m)
        {
            if (m != null)
            {
                this.propertyID = m.propertyID;
                this.name = m.name;
                this.formerName = m.formerName;
                this.streetAddress = m.streetAddress;
                this.city = m.city;
                this.market = m.market;
                this.state = m.state;
                this.lat = m.lat;
                this.lng = m.lng;
                this.Suggest = new CompletionField
                {
                    Input = new List<string>(this.name.Split(' ')) { this.name, this.formerName },
                    Weight = 1
                };
            }
        }
    }
    public class ApartmentBuildingDTO
    {
        public PropertyObj property { get; set; }
    }
}
