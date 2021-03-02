using System;
using Newtonsoft.Json;
using NodaTime;

namespace xggameplan.Model
{
    public class RecommendationModel
    {
        [JsonProperty(Order = -2)]
        public Guid ScenarioId { get; set; }

        public string ExternalSpotRef { get; set; }        // Spot number Spot.ExternalSpotRef

        public string ExternalCampaignNumber { get; set; }      // Campaign.ExternalId

        public string ExternalProgrammeReference { get; set; }

        public string ProgrammeName { get; set; }
        
        public Duration SpotLength { get; set; }

        public int Product { get; set; }   // Was ProductExternalIdentifier

        public int Demographic { get; set; }

        public int BreakBookingPosition { get; set; }

        public DateTime StartDateTime { get; set; }        // Was BreakDateTime

        //   public int SalesArea { get; set; }       // Spot sales area       SalesArea.CustomId

        public string SalesArea { get; set; }

        public int ProgrammeNo { get; set; }        // Programme.ProgrammeNo

        public string BreakType { get; set; }

        public double SpotRating { get; set; }

        public double SpotEfficiency { get; set; }

        // New below
        public double RatingPoints { get; set; }

        public string Action { get; set; }

        public string Processor { get; set; }

        public DateTime ProcessorDateTime { get; set; }

        public int GroupCode { get; set; }    // Set to same as SalesArea

        public DateTime EndDateTime { get; set; }       // Null/default

        public bool ClientPicked { get; set; }     // Lookup whether spot was hand-held or not (if handheld is true)

        public string MultipartSpot { get; set; }                     // # mpart_spot_ind

        public string MultipartSpotPosition { get; set; }            // # bkpo_posn_reqm

        public string MultipartSpotRef { get; set; }       // Empty string (Raise with Nine)

        public string RequestedPositionInBreak { get; set; }   // Map from bkpo_posn_reqm      # Empty string

        public string ActualPositionInBreak { get; set; }    // Empty string (Raise with Nine)

        public string ExternalBreakNo { get; set; }     // Look up from break information

        public bool Filler { get; set; }            // Hardcode false

        public bool Sponsored { get; set; }         // Hardcode false

        public bool Preemptable { get; set; }       // Hardcode false

        public int Preemptlevel { get; set; }       // Hardcode 0

        public int PassSequence { get; set; }       // Smooth pass

        public int PassIterationSequence { get; set; }    // Smooth pass iteration sequence

        public string PassName { get; set; }        // # abdn_no

        public int OptimiserPassSequenceNumber { get; set; }

        public int CampaignPassPriority { get; set; }

        public long RankOfEfficiency { get; set; }

        public int RankOfCampaign { get; set; }

        public double CampaignWeighting { get; set; }

        public long SpotSequenceNumber { get; set; }
    }


}
