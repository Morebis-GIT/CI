using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups.Objects;

namespace xggameplan.core.Services
{
    public interface IAnalysisGroupCampaignQuery
    {
        IEnumerable<Guid> GetAnalysisGroupCampaigns(AnalysisGroupFilter filter);
    }
}
