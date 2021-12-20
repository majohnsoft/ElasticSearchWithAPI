using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Smart.Business.Interface;
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

        [HttpGet]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> SearchAsync(string keyword, List<string> market)
        {
            try
            {
                var mgmtIndexName = _configuration.GetValue<string>("AppSettings:ManagementIndexName");
                var apartmentIndexName = _configuration.GetValue<string>("AppSettings:ApartmentIndexName");
                var limit = _configuration.GetValue<int>("AppSettings:Limit");
                var request = new SearchRequestModel()
                {
                    Keyword = keyword,
                    IndexName = mgmtIndexName,
                    Limit = limit,
                    Market = market
                };
                var searchResponse = await _mgmtServices.SearchAsync(request);
                request.IndexName = apartmentIndexName;
                searchResponse.AddRange(await _propertiesServices.SearchAsync(request));

                _logger.LogInformation($"Successful count {searchResponse.Count}");
                return Ok(searchResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new List<string>());
            }
        }

    }
}
