using Smart.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart.Objects.Model
{
    public class SearchResponseModel
    {
        public string name { get; set; }
        public string market { get; set; }
        public string state { get; set; }
        public bool IsApartment { get; set; }
    }

    
}
