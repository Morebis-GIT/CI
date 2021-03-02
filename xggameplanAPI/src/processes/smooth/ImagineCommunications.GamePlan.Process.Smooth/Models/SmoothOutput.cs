using System.Collections.Generic;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;

namespace ImagineCommunications.GamePlan.Process.Smooth.Models
{
    public class SmoothOutput
    {
        public string SalesAreaName { get; set; }

        public int Breaks { get; set; }

        public int SpotsSet { get; set; }

        public int Recommendations { get; set; }

        public int SpotsNotSet { get; set; }

        public int SpotsNotSetDueToExternalCampaignRef { get; set; }

        public int SpotsSetAfterMovingOtherSpots { get; set; }

        public int BookedSpotsUnplacedDueToRestrictions { get; set; }

        public int BreaksWithReducedOptimizerAvailForUnplacedSpots { get; set; }

        // Key = SmoothFailureMessage.Id
        public IDictionary<int, int> SpotsByFailureMessage = new Dictionary<int, int>();

        public IDictionary<int, SmoothOutputForPass> OutputByPass = new Dictionary<int, SmoothOutputForPass>();

        public int Failures { get; set; }

        /// <summary>
        /// Appends figures to this
        /// </summary>
        /// <param name="smoothOutput"></param>
        public void Append(SmoothOutput smoothOutput)
        {
            Breaks += smoothOutput.Breaks;
            Recommendations += smoothOutput.Recommendations;
            SpotsNotSet += smoothOutput.SpotsNotSet;
            SpotsSet += smoothOutput.SpotsSet;
            SpotsNotSetDueToExternalCampaignRef += smoothOutput.SpotsNotSetDueToExternalCampaignRef;
            SpotsSetAfterMovingOtherSpots += smoothOutput.SpotsSetAfterMovingOtherSpots;
            BookedSpotsUnplacedDueToRestrictions += smoothOutput.BookedSpotsUnplacedDueToRestrictions;
            BreaksWithReducedOptimizerAvailForUnplacedSpots += smoothOutput.BreaksWithReducedOptimizerAvailForUnplacedSpots;
            Failures += smoothOutput.Failures;

            if (SpotsByFailureMessage is null)
            {
                SpotsByFailureMessage = new Dictionary<int, int>();
            }

            // Append pass counts
            foreach (var passSequence in smoothOutput.OutputByPass.Keys)
            {
                if (!OutputByPass.ContainsKey(passSequence))
                {
                    OutputByPass.Add(passSequence, new SmoothOutputForPass() { PassSequence = passSequence });
                }

                OutputByPass[passSequence].CountSpotsSet += smoothOutput.OutputByPass[passSequence].CountSpotsSet;
            }

            //Append count of spots by failure message
            foreach (int failureMessageId in smoothOutput.SpotsByFailureMessage.Keys)
            {
                if (!SpotsByFailureMessage.ContainsKey(failureMessageId))
                {
                    SpotsByFailureMessage.Add(failureMessageId, 0);
                }

                SpotsByFailureMessage[failureMessageId] += smoothOutput.SpotsByFailureMessage[failureMessageId];
            }
        }
    }
}
