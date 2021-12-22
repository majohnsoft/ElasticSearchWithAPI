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
        public string Name { get; set; }
        public string Market { get; set; }
        public string State { get; set; }
        public bool IsApartment { get; set; }
        public double Score { get; set; }
        public static ApiResponse<T> SuccessResponse<T>(T data, string message = null)
        {
            return new ApiResponse<T>
            {

                description = message ?? "Success",
                payload = data,
                success = true,
                responseCode = ResponseCodesEnum.Success
            };
        }
        public static ApiResponse<T> FailedResponse<T>(string message = null)
        {
            return new ApiResponse<T>
            {

                description = message ?? "Failed",
                success = false,
                responseCode = ResponseCodesEnum.Failed
            };
        }
    }

    
}
