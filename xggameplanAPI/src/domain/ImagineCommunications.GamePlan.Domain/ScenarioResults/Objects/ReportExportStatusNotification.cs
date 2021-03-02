namespace ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects
{
    public class ReportExportStatusNotification
    {
        public string reportReference { get; set; }

        public ReportExportStatus status { get; set; }

        public string message { get; set; }
    }
}
