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
    public class ManagementServices : IManagementServices
    {
        readonly ElasticClient _elasticClient;

        public ManagementServices(ConnectionSettings connectionSettings)
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
                .Map<ManagementObj>(m => m
                .AutoMap()
                    .AutoMap()
                    .Properties(ps => ps
                        .Completion(c => c
                            .Name(p => p.Suggest)
                            .Analyzer("my_custom_analyzer")
                            .SearchAnalyzer("my_custom_analyzer")
                            .MaxInputLength(25)
                            .PreservePositionIncrements()
                            .PreserveSeparators()
                        )
                    )
                ));

            return createIndexResponse.IsValid;
        }

        public async Task AddManyAsync(string indexName, List<ManagementObj> mgmt)
        {
            await _elasticClient.IndexManyAsync(mgmt, indexName);
        }
        
        public async Task<ApiResponse<List<SearchResponseModel>>> SearchAsync(SearchRequestModel request)
        {
            if (string.IsNullOrEmpty(request.Keyword))
                return SearchResponseModel.FailedResponse<List<SearchResponseModel>>("Keyword is empty");
            if (request.Limit <= 0)
                return SearchResponseModel.FailedResponse<List<SearchResponseModel>>("Limit must be greater than 0");
            if (string.IsNullOrEmpty(request.IndexName))
                return SearchResponseModel.FailedResponse<List<SearchResponseModel>>("No Index name specified");


            IEnumerable<SearchResponseModel> searchSuggestion;
            if (request.Market != null)
            {
                var searchResponse = await _elasticClient.SearchAsync<ManagementObj>(s => s
                  .Index(request.IndexName)
                      .Suggest(su => su
                          .Completion("suggestions", c => c
                              .Field(f => f.Suggest)
                              .Prefix(request.Keyword)
                              .Fuzzy(f => f
                                  .Fuzziness(Fuzziness.Auto)
                              )
                              .Size(request.Limit))
                              )
                      );


                searchSuggestion = from suggest in searchResponse.Suggest["suggestions"]
                               from option in suggest.Options
                               where request.Market.Contains(option.Source.market)
                               select new SearchResponseModel
                               {
                                   Name = option.Source.name,
                                   Market = option.Source.market,
                                   State = option.Source.state,
                                   Score = option.Score,
                                   IsApartment = false
                               };
            }
            else
            {
                var searchResponse = await _elasticClient.SearchAsync<ManagementObj>(s => s
                  .Index(request.IndexName)
                      .Suggest(su => su
                          .Completion("suggestions", c => c
                              .Field(f => f.Suggest)
                              .Prefix(request.Keyword)
                              .Fuzzy(f => f
                                  .Fuzziness(Fuzziness.Auto)
                              )
                              .Size(request.Limit))
                              )
                      );


                searchSuggestion = from suggest in searchResponse.Suggest["suggestions"]
                                   from option in suggest.Options
                                   select new SearchResponseModel
                                   {
                                       Name = option.Source.name,
                                       Market = option.Source.market,
                                       State = option.Source.state,
                                       Score = option.Score,
                                       IsApartment = false
                                   };
            }
            if (searchSuggestion.Count() > 0)
            {
                return SearchResponseModel.SuccessResponse(searchSuggestion.ToList(), "Successful");
            }
            return SearchResponseModel.FailedResponse<List<SearchResponseModel>>("No search result found");
        }
    }
}
