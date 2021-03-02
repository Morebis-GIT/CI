using ImagineCommunications.GamePlan.Process.Smooth.Interfaces;
using ImagineCommunications.GamePlan.Process.Smooth.Services;

namespace ImagineCommunications.GamePlan.Process.Smooth.Dtos
{
    /// <summary>
    /// Resources for Smoothing spots. The eventual intention is to pass this
    /// class around instead of having to pass lots of parameters to methods.
    /// </summary>
    public class SmoothResources
    {
        public ICampaignClashChecker CampaignClashChecker { get; set; } = new CampaignClashChecker();
        public IClashExceptionChecker ClashExceptionChecker { get; set; } = new NullClashExceptionChecker();
        public IProductClashChecker ProductClashChecker { get; set; } = new ProductClashChecker();
        public IRestrictionChecker RestrictionChecker { get; set; } = new NullRestrictionChecker();
    }
}
