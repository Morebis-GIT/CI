using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.ApiConnectivity;
using xgCore.xgGamePlan.ApiEndPoints.Models.Metadata;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface IMetadataApi
    {
        [Get("/Metadata")]
        Task<List<MetadataValue>> GetByKey([Query] MetadataType key);

        [Post("/Metadata")]
        Task<ApiVersionResult> Create([Query] MetadataType key, List<string> values);

        [Delete("/Metadata")]
        Task<ApiVersionResult> Delete([Query] MetadataType key);
    }    
}
