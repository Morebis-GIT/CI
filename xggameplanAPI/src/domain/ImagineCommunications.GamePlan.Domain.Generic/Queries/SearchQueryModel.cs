namespace ImagineCommunications.GamePlan.Domain.Generic.Queries
{
    public class SearchQueryModel : BaseQueryModel
    {
        public string Title { get; set; }
        public OrderBy? OrderBy { get; set; }
        public OrderDirection? OrderDirection { get; set; }
    }
}
