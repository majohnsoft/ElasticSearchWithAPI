using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart.Data.Model
{
    public class ManagementObj
    {
        public int mgmtID { get; set; }
        public string name { get; set; }
        public string market { get; set; }
        public string state { get; set; }
        public CompletionField Suggest { get; set; }
        public ManagementObj(ManagementObj m)
        {
            var map = new Dictionary<string, IEnumerable<string>>();
            if (m != null)
            {
                this.mgmtID = m.mgmtID;
                this.name = m.name;
                this.market = m.market;
                this.state = m.state;
                this.Suggest = new CompletionField
                {
                    Input = new List<string>(this.name.Split(' ')) { this.name },
                    Weight = 1
                };
            }
        }
    }
    
    public class ManagementDTO
    {
        public ManagementObj mgmt { get; set; }
    }
}
