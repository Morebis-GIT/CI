using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Models;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    public abstract class SmoothPassExecuter
    {
        /// <summary>
        /// Gets spots matching filter criteria
        /// </summary>
        /// <param name="spotFilter"></param>
        /// <param name="spots"></param>
        /// <param name="spotInfos"></param>
        /// <returns></returns>
        protected IEnumerable<Spot> GetSpots(
            SpotFilter spotFilter,
            IReadOnlyCollection<Spot> spots,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos)
        {
            var spotsOutput = new List<Spot>(spots);

            // Remove specific spots to exclude
            _ = spotsOutput.RemoveAll(s => !s.ClientPicked);

            if (spotFilter.SpotIdsToExclude?.Count > 0)
            {
                _ = spotsOutput.RemoveAll(s => spotFilter.SpotIdsToExclude.Contains(s.Uid));
            }

            // Remove spots that don't meet criteria
            if (spotFilter.MinPreemptLevel != null)
            {
                _ = spotsOutput.RemoveAll(s => s.Preemptlevel < spotFilter.MinPreemptLevel);
            }

            if (spotFilter.MaxPreemptLevel != null)
            {
                _ = spotsOutput.RemoveAll(s => s.Preemptlevel > spotFilter.MaxPreemptLevel);
            }

            if (spotFilter.Sponsored != null)
            {
                _ = spotsOutput.RemoveAll(s => s.Sponsored != spotFilter.Sponsored);
            }

            if (spotFilter.Preemptable != null)
            {
                _ = spotsOutput.RemoveAll(s => s.Preemptable != spotFilter.Preemptable);
            }

            if (spotFilter.HasBreakRequest != null)
            {
                _ = spotsOutput.RemoveAll(s =>
                    (spotFilter.HasBreakRequest == true && String.IsNullOrEmpty(s.BreakRequest))
                    || (spotFilter.HasBreakRequest == false && !String.IsNullOrEmpty(s.BreakRequest))
                );
            }

            if (spotFilter.BreakRequests?.Count > 0)
            {
                _ = spotsOutput.RemoveAll(s =>
                    (!String.IsNullOrEmpty(s.BreakRequest) && !spotFilter.BreakRequests.Contains(s.BreakRequest))
                    || (String.IsNullOrEmpty(s.BreakRequest) && !spotFilter.BreakRequests.Contains(null))
                );
            }

            if (spotFilter.HasPositionInBreakRequest != null)
            {
                _ = spotsOutput.RemoveAll(s =>
                    (spotFilter.HasPositionInBreakRequest == true && String.IsNullOrEmpty(s.RequestedPositioninBreak))
                    || (spotFilter.HasPositionInBreakRequest == false && !String.IsNullOrEmpty(s.RequestedPositioninBreak))
                );
            }

            if (spotFilter.PositionInBreakRequestsToExclude?.Any() == true)
            {
                _ = spotsOutput.RemoveAll(s => spotFilter.PositionInBreakRequestsToExclude.Contains(s.RequestedPositioninBreak));
            }

            if (spotFilter.HasMultipartSpots != null)
            {
                _ = spotsOutput.RemoveAll(s =>
                    (spotFilter.HasMultipartSpots == true && !s.IsMultipartSpot)
                    || (spotFilter.HasMultipartSpots == false && s.IsMultipartSpot)
                );
            }

            if (spotFilter.MultipartSpots?.Count > 0)
            {
                _ = spotsOutput.RemoveAll(s => s.IsMultipartSpot && !spotFilter.MultipartSpots.Contains(s.MultipartSpot));
            }

            if (spotFilter.ExternalCampaignRefsToExclude?.Count > 0)
            {
                _ = spotsOutput.RemoveAll(s => spotFilter.ExternalCampaignRefsToExclude.Contains(s.ExternalCampaignNumber));
            }

            if (spotFilter.ProductClashCodesToExclude?.Count > 0)
            {
                _ = spotsOutput.RemoveAll(s => spotFilter.ProductClashCodesToExclude.Contains(spotInfos[s.Uid].ProductClashCode));
            }

            if (spotFilter.HasProductClashCode != null)
            {
                var spotsToRemove = new List<Spot>();

                foreach (var spot in spotsOutput)
                {
                    SpotInfo spotInfo = spotInfos[spot.Uid];

                    if ((spotFilter.HasProductClashCode == true && String.IsNullOrEmpty(spotInfo.ProductClashCode))
                        || (spotFilter.HasProductClashCode == false && !String.IsNullOrEmpty(spotInfo.ProductClashCode))
                        )
                    {
                        spotsToRemove.Add(spot);
                    }
                }

                if (spotsToRemove.Count > 0)
                {
                    _ = spotsOutput.RemoveAll(s => spotsToRemove.Contains(s));
                }
            }

            if (spotFilter.MinSpotLength != null)
            {
                _ = spotsOutput.RemoveAll(s => s.SpotLength.ToTimeSpan() < spotFilter.MinSpotLength);
            }

            if (spotFilter.MaxSpotLength != null)
            {
                _ = spotsOutput.RemoveAll(s => s.SpotLength.ToTimeSpan() > spotFilter.MaxSpotLength);
            }

            if (spotFilter.ProductAdvertiserIdentifiersToExclude?.Count > 0)
            {
                _ = spotsOutput.RemoveAll(s =>
                    spotFilter.ProductAdvertiserIdentifiersToExclude.Contains(spotInfos[s.Uid].ProductAdvertiserIdentifier)
                );
            }

            if (spotFilter.HasSpotEndTime != null)
            {
                if (spotFilter.HasSpotEndTime.Value)
                {
                    _ = spotsOutput.RemoveAll(s => s.EndDateTime < s.StartDateTime);
                }
                else
                {
                    _ = spotsOutput.RemoveAll(s => s.EndDateTime > s.StartDateTime);
                }
            }

            // Order will be set elsewhere
            return spotsOutput;
        }
    }
}
