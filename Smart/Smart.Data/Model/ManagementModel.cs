using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart.Data.Model
{
    public class ManagementInfo
    {
        public int mgmtID { get; set; }
        public string name { get; set; }
        public string market { get; set; }
        public string state { get; set; }
    }
    
    public class ManagementTerm
    {
        public string market { get; set; }
    }
    public class ManagementModel
    {
        public ManagementInfo mgmt { get; set; }
    }
}
