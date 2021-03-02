using ImagineCommunications.GamePlan.Domain.Generic.Queries;

namespace ImagineCommunications.GamePlan.Domain.Shared.Products.Queries
{
    public class AdvertiserSearchQueryModel : BaseQueryModel
    {
        /// <summary>
        /// Product have or start with this Advertiser Name or Externalidentifier will be returned
        /// </summary>
        public string AdvertiserNameorRef { get; set; }

        /// <summary>
        /// If specified, response includes actual TotalCount, otherwise 0
        /// </summary>
        public bool IncludeTotalCount { get; set; } 
    }
}
