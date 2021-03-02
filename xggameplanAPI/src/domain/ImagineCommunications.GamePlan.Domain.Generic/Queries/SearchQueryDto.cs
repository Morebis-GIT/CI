namespace ImagineCommunications.GamePlan.Domain.Generic.Queries
{
    public class SearchQueryDto
    {
        private readonly int defaultSkipValue = 0;
        private readonly int defaultTopValue = int.MaxValue;

        public OrderBy OrderBy { get; }
        public OrderDirection OrderDirection { get; }
        public int Skip { get; }
        public string Title { get; }
        public int Top { get; }

        private SearchQueryDto()
        {
            OrderBy = OrderBy.Title;
            OrderDirection = OrderDirection.Asc;
            Skip = defaultSkipValue;
            Title = string.Empty;
            Top = defaultTopValue;
        }

        public SearchQueryDto(SearchQueryModel query) : this()
        {
            if (query is null)
            {
                return;
            }

            OrderBy = query.OrderBy ?? OrderBy.Title;
            OrderDirection = query.OrderDirection ?? OrderDirection.Asc;
            Skip = query.Skip ?? defaultSkipValue;
            Title = query.Title;
            Top = query.Top ?? defaultTopValue;
        }
    }
}