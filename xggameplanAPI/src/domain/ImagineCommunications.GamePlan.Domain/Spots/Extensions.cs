namespace ImagineCommunications.GamePlan.Domain.Spots
{
    public static class Extensions
    {
        public static bool IsBooked(this Spot spot) =>
            SpotHelper.IsBooked(spot?.ExternalBreakNo);
    }
}
