namespace ImagineCommunications.GamePlan.Domain.SmoothConfigurations
{
    /// <summary>
    /// Defines rules for positioning spot. It can be used for defining which break and which position in the break
    /// </summary>
    public enum SpotPositionRules : byte
    {
        Exact = 0,            // In position requested
        Near = 1,             // In position N either side of requested position
        Anywhere = 2
    }
}
