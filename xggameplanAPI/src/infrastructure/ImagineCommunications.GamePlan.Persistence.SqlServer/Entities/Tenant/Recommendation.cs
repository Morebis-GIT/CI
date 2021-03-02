using System;
using System.ComponentModel.DataAnnotations;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class Recommendation : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid ScenarioId { get; set; }
        public string ExternalSpotRef { get; set; }
        public string ExternalCampaignNumber { get; set; }
        public TimeSpan SpotLength { get; set; }
        public string Product { get; set; }
        public string Demographic { get; set; }
        public int BreakBookingPosition { get; set; }
        public DateTime StartDateTime { get; set; }
        public string SalesArea { get; set; }
        public string ExternalProgrammeReference { get; set; }

        [MaxLength(128)]
        public string ProgrammeName { get; set; }

        public string BreakType { get; set; }
        public double NominalPrice { get; set; }
        public decimal SpotRating { get; set; }
        public double SpotEfficiency { get; set; }
        public double RatingPoints { get; set; }
        public string Action { get; set; }
        public string Processor { get; set; }
        public DateTime ProcessorDateTime { get; set; }
        public string GroupCode { get; set; }
        public DateTime EndDateTime { get; set; }
        public bool ClientPicked { get; set; }
        public string MultipartSpot { get; set; }
        public string MultipartSpotPosition { get; set; }
        public string MultipartSpotRef { get; set; }
        public string RequestedPositionInBreak { get; set; }
        public string ActualPositionInBreak { get; set; }
        public string ExternalBreakNo { get; set; }
        public bool Filler { get; set; }
        public bool Sponsored { get; set; }
        public bool Preemptable { get; set; }
        public int Preemptlevel { get; set; }
        public int PassSequence { get; set; }
        public int PassIterationSequence { get; set; }
        public string PassName { get; set; }
        public int OptimiserPassSequenceNumber { get; set; }
        public int CampaignPassPriority { get; set; }
        public long RankOfEfficiency { get; set; }
        public int RankOfCampaign { get; set; }
        public double CampaignWeighting { get; set; }
        public long SpotSequenceNumber { get; set; }
    }
}
