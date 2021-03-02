namespace xgCore.xgGamePlan.ApiEndPoints.Models.AutoBook
{
    public class UpdateAutoBookModel
    {
        public string Api { get; set; }

        public AutoBookStatuses Status { get; set; }

        public AutoBookTaskModel Task { get; set; }
    }
}
