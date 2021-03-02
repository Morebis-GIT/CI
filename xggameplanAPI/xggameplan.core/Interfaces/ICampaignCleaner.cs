using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;

namespace xggameplan.core.Interfaces
{
    public interface ICampaignCleaner
    {
        Task ExecuteAsync(CancellationToken cancellationToken = default);

        Task ExecuteAsync(string externalId, CancellationToken cancellationToken = default);

        Task ExecuteAsync(IReadOnlyCollection<string> externalIds, CancellationToken cancellationToken = default);

        Task ExecuteAsync(Guid campaignId, CancellationToken cancellationToken = default);

        Task ExecuteAsync(IReadOnlyCollection<Guid> campaignIds, CancellationToken cancellationToken = default);

        Task ExecuteAsync(IReadOnlyCollection<Campaign> campaigns, CancellationToken cancellationToken = default);
    }
}
