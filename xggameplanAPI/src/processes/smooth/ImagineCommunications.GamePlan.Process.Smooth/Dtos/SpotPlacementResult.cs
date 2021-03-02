namespace ImagineCommunications.GamePlan.Process.Smooth.Dtos
{
    public struct SpotPlacementResult
    {
        public int CountValidBreaks { get; set; }

        public BestBreakResult BestBreakResult { get; set; }

        public PlaceSpotsResult PlaceSpotsResult { get; set; }
    }
}
