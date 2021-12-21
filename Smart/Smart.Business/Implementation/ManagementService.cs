using Nest;
using Smart.Business.Interface;
using Smart.Data.Model;
using Smart.Objects.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Smart.Business.Implementation
{
    public class ManagementService : IManagementService
    {
        readonly ElasticClient _elasticClient;

        public ManagementService(ConnectionSettings connectionSettings)
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

            var createIndexResponse = _elasticClient.Indices.Create(indexName, c => c
                    .Settings(s => s
                        .Analysis(a => a
                            .Analyzers(an => an
                                .Custom("my_custom_analyzer", mca => mca
                                    .Tokenizer("standard")
                                    .Filters("lowercase", "stop")
                                )
                            )
                        )
                    )
                    .Map<ManagementModel>(m => m
                        .AutoMap()
                        .Properties(ps => ps
                            .Text(c => c
                                .Name(p => p.mgmt.market)
                                .Name(p => p.mgmt.name)
                                .Name(p => p.mgmt.state)
                                .Analyzer("my_custom_analyzer"))
                        )
                    )
                );

            return createIndexResponse.IsValid;
        }

        public async Task AddManyAsync(string indexName, List<ManagementModel> mgmt)
        {
            await _elasticClient.IndexManyAsync(mgmt, indexName);
        }
        
        public async Task<List<SearchResponseModel>> SearchAsync(SearchRequestModel request)
        {
            if (string.IsNullOrEmpty(request.Keyword))
                return new List<SearchResponseModel>();


            ISearchResponse<ManagementModel> searchResponse;
            if (request.Market != null)
            {
                var marketsFilter = request.Market.Select(value =>
                {
                    Func<QueryContainerDescriptor<ManagementModel>, QueryContainer> marketFilter = filter => filter
                        .Term(tm => tm.mgmt.market.ToLowerInvariant(), value.ToLowerInvariant());
                    return marketFilter;
                });

                searchResponse = await _elasticClient.SearchAsync<ManagementModel>(s => s
                    .Index(request.IndexName)
                    .Size(request.Limit)
                    .Query(q => q
                        .Bool(b => b
                            .Must(m => m
                                .Match(t => t
                                    .Query(request.Keyword)
                                    .Field(f => f.mgmt.name)
                                )
                            )
                            .Filter(marketsFilter)
                        )
                    )
                );
            }
            else
            {
                searchResponse = await _elasticClient.SearchAsync<ManagementModel>(s => s
                    .Index(request.IndexName)
                    .Size(request.Limit)
                    .Query(q => q
                        .Bool(b => b
                            .Must(m => m
                                .Match(t => t
                                    .Query(request.Keyword)
                                    .Field(f => f.mgmt.name)
                                )
                            )
                        )
                    )
                );
            }

            var _list = searchResponse.Hits.Select(s => s.Source).ToList();
            if (_list.Count > 0)
            {
                var suggests = _list.GroupBy(g => new { g.mgmt.name, g.mgmt.market, g.mgmt.state }).Select(s => new SearchResponseModel()
                {
                    name = s.Key.name,
                    market = s.Key.market,
                    state = s.Key.state,
                    IsApartment = false
                }).ToList();


                return suggests;
            }
            return new List<SearchResponseModel>();
        }
    }
}
