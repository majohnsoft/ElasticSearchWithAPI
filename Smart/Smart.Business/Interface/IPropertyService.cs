using Smart.Data.Model;
using Smart.Objects.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Smart.Business.Interface
{
    public interface IPropertyService
    {
        Task AddManyAsync(string indexName, List<PropertyModel> properties);
        Task<bool> CreateIndexAsync(string indexName);
        Task<List<SearchResponseModel>> SearchAsync(SearchRequestModel request);
    }
}