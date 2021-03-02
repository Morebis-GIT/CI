namespace xggameplan.core.Landmark
{
    public class LandmarkApiConfig
    {
        public LandmarkInstanceConfig Primary { get; set; }
        public LandmarkInstanceConfig Secondary { get; set; }
        public LandmarkInstanceConfig DefaultInstance => Primary;

        public void ApplyDefaultsToSecondaryInstance() => Secondary.UpdateOptionalValues(Primary);
    }
}
