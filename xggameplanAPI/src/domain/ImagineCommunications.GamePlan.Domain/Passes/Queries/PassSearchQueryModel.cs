using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;

namespace ImagineCommunications.GamePlan.Domain.Passes.Queries
{
    /// <summary>
    /// Model for searching passes
    /// </summary>
    public class PassSearchQueryModel : BaseQueryModel
    {
        /// <summary>
        /// Name to search for
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Whether to include libraried/not-libraried (null=Any)
        /// </summary>
        public bool? IsLibraried { get; set; }

        /// <summary>
        /// Sort order
        /// </summary>
        public List<Order<string>> OrderBy { get; set; }
    }
}
