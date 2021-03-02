using ImagineCommunications.GamePlan.Domain.Generic.Queries;

namespace ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Queries
{
    public class ClashSearchQueryModel : BaseQueryModel
    {
        /// <summary>
        /// clash having or starting this clash code or description will be returned
        /// </summary>
        public string NameOrRef { get; set; }

    }
}
