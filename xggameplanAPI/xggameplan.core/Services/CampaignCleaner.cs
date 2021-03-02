using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using xggameplan.core.Interfaces;

namespace xggameplan.core.Services
{
    public class CampaignCleaner : ICampaignCleaner
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly IScenarioRepository _scenarioRepository;

        public CampaignCleaner(
            ICampaignRepository campaignRepository,
            IScenarioRepository scenarioRepository)
        {
            _campaignRepository = campaignRepository;
            _scenarioRepository = scenarioRepository;
        }

        public Task ExecuteAsync(Guid campaignId, CancellationToken cancellationToken = default)
        {
            _campaignRepository.Delete(campaignId);
            _campaignRepository.SaveChanges();

            var updated = false;
            foreach (var scenario in _scenarioRepository.GetLibraried())
            {
                if ((scenario.CampaignPassPriorities?.RemoveAll(x =>
                    !(x.Campaign is null) && x.Campaign.Uid == campaignId) ?? 0) > 0)
                {
                    _scenarioRepository.Update(scenario);
                    updated = true;
                }
            }

            if (updated)
            {
                _scenarioRepository.SaveChanges();
            }

            return Task.CompletedTask;
        }

        public Task ExecuteAsync(IReadOnlyCollection<Guid> campaignIds, CancellationToken cancellationToken = default)
        {
            if (campaignIds is null || campaignIds.Count == 0)
            {
                return Task.CompletedTask;
            }

            var externalIds = _campaignRepository
                .Find(campaignIds.ToList())
                .Select(x => x.ExternalId)
                .ToArray();

            return ExecuteAsync(externalIds, cancellationToken);
        }

        public Task ExecuteAsync(IReadOnlyCollection<Campaign> campaigns, CancellationToken cancellationToken = default)
        {
            if (campaigns is null || campaigns.Count == 0)
            {
                return Task.CompletedTask;
            }

            return ExecuteAsync(campaigns.Select(x => x.ExternalId).ToArray(), cancellationToken);
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            await _campaignRepository.TruncateAsync().ConfigureAwait(false);

            foreach (var scenario in _scenarioRepository.GetLibraried())
            {
                scenario.CampaignPassPriorities?.Clear();
                _scenarioRepository.Update(scenario);
            }

            _scenarioRepository.SaveChanges();
        }

        public Task ExecuteAsync(string externalId, CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(new[] { externalId }, cancellationToken);
        }

        public Task ExecuteAsync(IReadOnlyCollection<string> externalIds, CancellationToken cancellationToken = default)
        {
            if (externalIds is null || externalIds.Count == 0)
            {
                return Task.CompletedTask;
            }

            _campaignRepository.Delete(externalIds);
            _campaignRepository.SaveChanges();

            var removed = false;
            foreach (var scenario in _scenarioRepository.GetLibraried())
            {
                removed |= (scenario.CampaignPassPriorities?.RemoveAll(x =>
                    !(x.Campaign is null) && externalIds.Contains(x.Campaign.ExternalId)) ?? 0) > 0;
            }

            if (removed)
            {
                _scenarioRepository.SaveChanges();
            }

            return Task.CompletedTask;
        }
    }
}
