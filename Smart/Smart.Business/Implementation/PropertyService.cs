using Microsoft.Extensions.Logging;
using Nest;
using Smart.Business.Interface;
using Smart.Data.Model;
using Smart.Objects.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart.Business.Implementation
{
    public class PropertyService : IPropertyService
    {
        readonly ElasticClient _elasticClient;
        private ILogger<PropertyService> _logger;

        public PropertyService(ConnectionSettings connectionSettings, ILogger<PropertyService> logger)
        {
            _elasticClient = new ElasticClient(connectionSettings);
            _logger = logger;
        }

        public async Task<bool> CreateIndexAsync(string indexName)
        {
            try
            {
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
                    .Map<PropertyModel>(m => m
                        .AutoMap()
                        .Properties(ps => ps
                            .Text(c => c
                                .Name(p => p.property.market)
                                .Name(p => p.property.name)
                                .Name(p => p.property.streetAddress)
                                .Name(p => p.property.city)
                                .Name(p => p.property.state)
                                .Analyzer("my_custom_analyzer"))
                        )
                    )
                );

                //var createIndexResponse = _elasticClient.Indices.Create(indexName, ms => ms
                //        .Map<PropertyModel>(m => m
                //            .AutoMap()
                //                .AutoMap()
                //                    .Properties(ps => ps
                //                        .Completion(c => c
                //                            .Name(p => p.property.market)
                //                            .Name(p => p.property.name)
                //                            .Name(p => p.property.formerName)))
                //            ));

                return createIndexResponse.IsValid;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }

        public async Task AddManyAsync(string indexName, List<PropertyModel> properties)
        {
            await _elasticClient.IndexManyAsync(properties, indexName);
        }
        
        public async Task<List<SearchResponseModel>> SearchAsync(SearchRequestModel request)
        {

            if (string.IsNullOrEmpty(request.Keyword))
                request.Keyword = string.Empty;


            ISearchResponse<PropertyModel> searchResponse;
            if (request.Market.Count > 0)
            {
                var marketsFilter = request.Market.Select(value =>
                {
                    Func<QueryContainerDescriptor<PropertyModel>, QueryContainer> marketFilter = filter => filter
                        .Term(tm => tm.property.market.ToLowerInvariant(), value.ToLowerInvariant());
                    return marketFilter;
                });

                searchResponse = await _elasticClient.SearchAsync<PropertyModel>(s => s
                    .Index(request.IndexName)
                    .Size(request.Limit)
                    .Query(q => q
                        .Bool(b => b
                            .Must(m => m
                                .MultiMatch(t => t
                                    .Query(request.Keyword)
                                    .Fields(f => f
                                        .Fields(f1 =>
                                            f1.property.name,
                                            f2 => f2.property.formerName
                                        )
                                    )
                                )
                            )
                            .Filter(marketsFilter)
                        )
                    )
                );
            }
            else
            {
                searchResponse = await _elasticClient.SearchAsync<PropertyModel>(s => s
                    .Index(request.IndexName)
                    .Size(request.Limit)
                    .Query(q => q
                        .Bool(b => b
                            .Must(m => m
                                .Match(t => t
                                    .Query(request.Keyword)
                                    .Field(f => f.property.name)
                                )
                            )
                        )
                    )
                );
            }

            var _list = searchResponse.Hits.Select(s => s.Source).ToList();
            if (_list.Count > 0)
            {
                var suggests = _list.Distinct().Select(s => new SearchResponseModel()
                {
                    name = s.property.name,
                    market = s.property.market,
                    state = s.property.state,
                    IsApartment = true
                }).ToList();
                return suggests;
            }
            return new List<SearchResponseModel>();
        }
    }
}
