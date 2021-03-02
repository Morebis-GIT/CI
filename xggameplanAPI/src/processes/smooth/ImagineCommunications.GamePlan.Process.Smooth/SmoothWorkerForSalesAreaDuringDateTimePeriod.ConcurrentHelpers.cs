using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using xggameplan.Extensions;

namespace ImagineCommunications.GamePlan.Process.Smooth
{
    public partial class SmoothWorkerForSalesAreaDuringDateTimePeriod
    {
        private static (HashSet<Guid> spotIdsUsed, HashSet<Guid> spotIdsNotUsed)
        CollateThreadOutputToSmoothOutput(
            SmoothOutput smoothOutput,
            ConcurrentBag<SmoothBatchOutput> batchAllThreadOutputCollection)
        {
            var spotIdsUsed = new HashSet<Guid>();
            var spotIdsNotUsed = new HashSet<Guid>();

            foreach (var batch in batchAllThreadOutputCollection)
            {
                batch.UnusedSpotIds.CopyDistinctTo(spotIdsNotUsed);
                batch.UsedSpotIds.CopyDistinctTo(spotIdsUsed);

                foreach (KeyValuePair<int, SmoothOutputForPass> item in batch.OutputByPass)
                {
                    var (smoothPassSequence, passOutput) = (item.Key, item.Value);

                    if (smoothOutput.OutputByPass.ContainsKey(smoothPassSequence))
                    {
                        smoothOutput.OutputByPass[smoothPassSequence].CountSpotsSet += passOutput.CountSpotsSet;
                    }
                    else
                    {
                        var smoothPassOutput = new SmoothOutputForPass
                        {
                            CountSpotsSet = passOutput.CountSpotsSet,
                            PassSequence = passOutput.PassSequence
                        };

                        smoothOutput.OutputByPass.Add(smoothPassSequence, smoothPassOutput);
                    }
                }

                foreach (KeyValuePair<int, int> item in batch.SpotsByFailureMessage)
                {
                    var (messageId, count) = (item.Key, item.Value);

                    if (smoothOutput.SpotsByFailureMessage.ContainsKey(messageId))
                    {
                        smoothOutput.SpotsByFailureMessage[messageId] += count;
                    }
                    else
                    {
                        smoothOutput.SpotsByFailureMessage.Add(messageId, count);
                    }
                }

                smoothOutput.BookedSpotsUnplacedDueToRestrictions += batch.BookedSpotsUnplacedDueToRestrictions;
                smoothOutput.Breaks += batch.Breaks;
                smoothOutput.Failures += batch.Failures;
                smoothOutput.Recommendations += batch.Recommendations;
                smoothOutput.SpotsNotSetDueToExternalCampaignRef += batch.SpotsNotSetDueToExternalCampaignRef;
                smoothOutput.SpotsSetAfterMovingOtherSpots += batch.SpotsSetAfterMovingOtherSpots;
                smoothOutput.SpotsSet += batch.SpotsSet;
            }

            return (spotIdsUsed, spotIdsNotUsed);
        }
    }
}
