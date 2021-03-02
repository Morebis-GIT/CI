using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;

namespace ImagineCommunications.GamePlan.Domain.Scenarios.Queries
{
    /// <summary>
    /// Model for searching scenarios
    /// </summary>
    public class ScenarioSearchQueryModel : BaseQueryModel
    {
        /// <summary>
        /// Name to search for
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Whether default scenario is at the top of the first page, typically
        /// for paging queries. Pass the default scenario id in the
        /// <see cref="DefaultScenarioId"/> property.
        /// </summary>
        public bool DefaultScenarioAtTopOfFirstPage { get; set; }

        /// <summary>
        /// Whether to include libraried/not-libraried (null=Any)
        /// </summary>
        public bool? IsLibraried { get; set; }

        /// <summary>
        /// Sort order
        /// </summary>
        public List<Order<string>> OrderBy { get; set; }

        /// <summary>
        /// The Id of the default scenario. Set this when requesting the
        /// default scenario to be at the top of the first page.
        /// </summary>
        public Guid? DefaultScenarioId { get; set; }
    }
}
