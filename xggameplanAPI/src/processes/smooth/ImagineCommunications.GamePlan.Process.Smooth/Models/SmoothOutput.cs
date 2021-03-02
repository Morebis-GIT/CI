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

            // Append pass counts
            foreach (var passSequence in smoothOutput.OutputByPass.Keys)
            {
                int outputCountSpotsSet = smoothOutput.OutputByPass[passSequence].CountSpotsSet;
                if (OutputByPass.ContainsKey(passSequence))
                {
                    OutputByPass[passSequence].CountSpotsSet += outputCountSpotsSet;
                }
                else
                {
                    var outputForPass = new SmoothOutputForPass(passSequence)
                    {
                        CountSpotsSet = outputCountSpotsSet
                    };

                    OutputByPass.Add(passSequence, outputForPass);
                }
            }

            // Append count of spots by failure message
            if (SpotsByFailureMessage is null)
            {
                SpotsByFailureMessage = new Dictionary<int, int>();
            }

            foreach (int failureMessageId in smoothOutput.SpotsByFailureMessage.Keys)
            {
                int value = smoothOutput.SpotsByFailureMessage[failureMessageId];
                if (SpotsByFailureMessage.ContainsKey(failureMessageId))
                {
                    SpotsByFailureMessage[failureMessageId] += value;
                }
                else
                {
                    SpotsByFailureMessage.Add(failureMessageId, value);
                }
            }
        }
    }
}
