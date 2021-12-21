using Microsoft.Extensions.Logging;
using Nest;
using Smart.Business.Interface;
using Smart.Objects.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart.Business.Implementation
{
    public class MarketRegionService : IMarketRegionService
    {
        readonly ElasticClient _elasticClient;

        public MarketRegionService(ConnectionSettings connectionSettings)
        {
            _elasticClient = new ElasticClient(connectionSettings);
        }

        public async Task<bool> CreateIndexAsync(string indexName)
        {
            if (string.IsNullOrEmpty(indexName))
                return false;
            var isExist = await _elasticClient.Indices.ExistsAsync(indexName.ToLowerInvariant());
            if (isExist.Exists)
            {
                _elasticClient.Indices.Delete(indexName.ToLowerInvariant());
            }

            var createIndexResponse = _elasticClient.Indices.Create(indexName, ms => ms
                    .Map<MarketRegion>(m => m
                        .AutoMap()
                            .AutoMap()
                                .Properties(ps => ps
                                    .Completion(c => c
                                        .Name(p => p.market)
                                        .Name(p => p.state)))
                        ));

            return createIndexResponse.IsValid;

        }

        public async Task AddManyAsync(string indexName, List<MarketRegion> region)
        {
            await _elasticClient.IndexManyAsync(region, indexName);
        }

        public async Task<List<MarketRegion>> GetAllRegionAsync(string indexName)
        {
            var searchResponse = await _elasticClient.SearchAsync<MarketRegion>(s => s
                    .Index(indexName)
                        .Query(q => q
                            .MatchAll()
                        )
                        .Size(100)
                        );

            return searchResponse.Hits.Select(s => s.Source).ToList();
        }
    }
}
