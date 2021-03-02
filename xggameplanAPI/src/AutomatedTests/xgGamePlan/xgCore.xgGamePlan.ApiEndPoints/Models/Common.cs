namespace xgCore.xgGamePlan.ApiEndPoints.Models
{
    public enum IncludeOrExclude
    {
        I = 0,
        E = 1
    }

    public enum IncludeOrExcludeOrEither
    {
        X = 0,
        I = 1,
        E = 2,
    }

    public enum IncludeRightSizer
    {
        No = 0,
        CampaignLevel = 1,
        DetailLevel = 2
    }

    public enum ForceOverUnder
    {
        None = 0,
        Under = 1,
        Over = 2
    }

    public class Order<T>
    {
        public T OrderBy { get; set; } 
        public OrderDirection OrderDirection { get; set; }
    }

    public enum OrderDirection
    {
        Asc,
        Desc
    }
}
