namespace xggameplan.common.Search
{
    public abstract class SearchConditionItem<TResult> : IBuildAction<TResult>
    {
        public SearchLogicalOperand? LogicalOperand { get; set; }

        public abstract TResult Build();
    }
}
