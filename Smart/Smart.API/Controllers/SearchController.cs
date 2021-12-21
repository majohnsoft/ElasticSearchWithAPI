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
        private readonly IManagementService _mgmtServices;
        private readonly IPropertyService _propertiesServices;
        private IConfiguration _configuration { get; }
        private ILogger<SearchController> _logger;

        public SearchController(IManagementService mgmtServices, IPropertyService propertiesServices, IConfiguration configuration,
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
            var mgmtIndexName = Constants.MANAGEMENTCOMPANYINDEXNAME;
            var apartmentIndexName = Constants.APARTMENTINDEXNAME;

            request.IndexName = mgmtIndexName;
            var searchResponse = await _mgmtServices.SearchAsync(request);
            request.IndexName = apartmentIndexName;
            searchResponse.AddRange(await _propertiesServices.SearchAsync(request));

            _logger.LogInformation($"Successful count {searchResponse.Count}");
            return Ok(searchResponse);
        }

    }
}
