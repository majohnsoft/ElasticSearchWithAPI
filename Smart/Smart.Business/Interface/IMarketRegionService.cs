using Smart.Objects.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Smart.Business.Interface
{
    public interface IMarketRegionService
    {
        Task AddManyAsync(string indexName, List<MarketRegion> properties);
        Task<bool> CreateIndexAsync(string indexName);
        Task<List<MarketRegion>> GetAllRegionAsync(string indexName);
    }
}