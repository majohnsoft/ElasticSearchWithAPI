using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart.Objects.Model
{
    public class SearchRequestModel
    {
        public string IndexName { get; set; }
        public string Keyword { get; set; }
        public int Limit { get; set; }
        public List<string> Market { get; set; }
    }
}