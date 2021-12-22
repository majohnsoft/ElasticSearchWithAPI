using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart.Objects.Model
{
    public class ApiResponse<T>
    {
        public T payload { get; set; } = default(T);
        public bool success { get; set; }
        public string description { get; set; }
        public ResponseCodesEnum responseCode { get; set; }
    }
}
