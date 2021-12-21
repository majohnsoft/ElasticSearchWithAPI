using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Smart.Business.Interface;
using Smart.Data.Model;
using Smart.Objects;
using Smart.Objects.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smart.API.Controllers
{
    [Route("api/market")]
    public class MarketController : Controller
    {
        private readonly IMarketRegionService _regionServices;
        private IConfiguration _configuration { get; }

        private ILogger<MarketController> _logger;

        public MarketController(IMarketRegionService regionServices, IConfiguration configuration,
                                ILogger<MarketController> logger)
        {
            _regionServices = regionServices;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> GetDistinctMarket()
        {
            var regionIndexName = Constants.MARKETINDEXNAME;
            var distinctMgmtMarket = await _regionServices.GetAllRegionAsync(regionIndexName);
            _logger.LogInformation($"Successful count {distinctMgmtMarket.Count}");

            return Ok(distinctMgmtMarket);
        }
    }
}
