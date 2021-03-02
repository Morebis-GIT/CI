namespace ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects
{
    public enum ReportExportStatus
    {
        Requested = 0,
        Queued = 1,
        Generating = 2,
        GenerationFailed = 3,
        Uploading = 4,
        UploadFailed = 5,
        Available = 6
    }
}
