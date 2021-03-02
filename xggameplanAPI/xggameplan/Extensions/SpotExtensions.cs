using ImagineCommunications.GamePlan.Domain.Spots;
using xggameplan.Common;
using xggameplan.Model;

namespace xggameplan.Extensions
{
    public static class SpotExtensions
    {
        public static bool IsTopMultipart(this Spot spot) =>
            spot.MultipartSpot == MultipartSpotTypes.TopTail &&
            spot.MultipartSpotPosition == MultipartSpotPositions.TopTail_Top ||
            spot.MultipartSpot == MultipartSpotTypes.SameBreak &&
            (spot.MultipartSpotPosition == MultipartSpotPositions.SameBreak_Top ||
             spot.MultipartSpotPosition == MultipartSpotPositions.SameBreak_Any);

        public static void Update(this Spot spot, CreateSpot createSpot)
        {
            spot.ExternalCampaignNumber = createSpot.ExternalCampaignNumber;
            spot.SalesArea = createSpot.SalesArea;
            spot.GroupCode = createSpot.GroupCode;
            spot.ExternalSpotRef = createSpot.ExternalSpotRef;
            spot.StartDateTime = createSpot.StartDateTime;
            spot.EndDateTime = createSpot.EndDateTime;
            spot.SpotLength = createSpot.SpotLength;
            spot.BreakType = createSpot.BreakType;
            spot.Product = createSpot.Product;
            spot.Demographic = createSpot.Demographic;
            spot.ClientPicked = createSpot.ClientPicked;
            spot.MultipartSpot = createSpot.MultipartSpot;
            spot.MultipartSpotPosition = createSpot.MultipartSpotPosition;
            spot.MultipartSpotRef = createSpot.MultipartSpotRef;
            spot.RequestedPositioninBreak = createSpot.RequestedPositioninBreak;
            spot.ActualPositioninBreak = createSpot.ActualPositioninBreak;
            spot.BreakRequest = createSpot.BreakRequest;
            spot.ExternalBreakNo = createSpot.ExternalBreakNo;
            spot.Sponsored = createSpot.Sponsored;
            spot.Preemptable = createSpot.Preemptable;
            spot.Preemptlevel = createSpot.Preemptlevel;
            spot.IndustryCode = createSpot.IndustryCode;
            spot.ClearanceCode = createSpot.ClearanceCode;
        }
    }
}
