using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.KPIComparisonConfigs {
    public interface IKPIComparisonConfigRepository
    {
        List<KPIComparisonConfig> GetAll();
    }
}