using Smart.Data.Model;
using Smart.Objects.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Smart.Business.Interface
{
    public interface IPropertyServices
    {
        Task AddManyAsync(string indexName, List<PropertyObj> properties);
        Task<bool> CreateIndexAsync(string indexName);
        Task<ApiResponse<List<SearchResponseModel>>> SearchAsync(SearchRequestModel request);
    }
}