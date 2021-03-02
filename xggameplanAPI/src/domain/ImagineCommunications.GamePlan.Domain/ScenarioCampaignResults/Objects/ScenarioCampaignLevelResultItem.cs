namespace ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults.Objects
{
    public class ScenarioCampaignLevelResultItem
    {
        public int PeriodSubmissionNumber { get; set; }

        public int CampaignNumber { get; set; }

        public string CampaignExternalId { get; set; }

        public double TargetRatings { get; set; }

        public double PreRunRatings { get; set; }

        public double ISRCancelledRatings { get; set; }

        public double ISRCancelledSpots { get; set; }

        public double RSCancelledRatings { get; set; }

        public double RSCancelledSpots { get; set; }

        public double OptimiserRatings { get; set; }

        public double OptimiserBookedSpots { get; set; }

        public double ZeroRatedSpots { get; set; }

        public double NominalValue { get; set; }
    }
}
