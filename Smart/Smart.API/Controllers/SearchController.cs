using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Smart.Business.Interface;
using Smart.Objects;
using Smart.Objects.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smart.API.Controllers
{
    [Route("api/search")]
    public class SearchController : Controller
    {
        // GET: SearchController
        private readonly IManagementServices _mgmtServices;
        private readonly IPropertyServices _propertiesServices;
        private IConfiguration _configuration { get; }
        private ILogger<SearchController> _logger;

        public SearchController(IManagementServices mgmtServices, IPropertyServices propertiesServices, IConfiguration configuration,
                        ILogger<SearchController> logger)
        {
            _mgmtServices = mgmtServices;
            _propertiesServices = propertiesServices;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        [EnableCors("AllowOrigin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Produces("application/json")]
        public async Task<IActionResult> SearchAsync([FromBody] SearchRequestModel request)
        {
            var propertyIndexName = Constants.APARTMENTINDEXNAME;
            var managementIndexName = Constants.MANAGEMENTCOMPANYINDEXNAME;

            request.IndexName = propertyIndexName;
            var apartmentResponse = await _propertiesServices.SearchAsync(request);

            request.IndexName = managementIndexName;
            var managementResponse = await _mgmtServices.SearchAsync(request);


            var response = new ApiResponse<List<SearchResponseModel>>();
            if (apartmentResponse.success && managementResponse.success)
            {
                response = apartmentResponse;
                response.payload.AddRange(managementResponse.payload);
            }
            else if (apartmentResponse.success && !managementResponse.success)
                response = apartmentResponse;
            else if (!apartmentResponse.success && managementResponse.success)
                response = managementResponse;
            else
                response = SearchResponseModel.FailedResponse<List<SearchResponseModel>>("No Search Result");

            _logger.LogInformation($" is Successful {response.success}");
            return Ok(response);
        }

    }
}
