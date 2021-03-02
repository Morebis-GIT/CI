using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class CampaignSettingsDomainModelHandler : IDomainModelHandler<CampaignSettings>
    {
        private readonly ICampaignSettingsRepository _campaignSettingsRepository;

        public CampaignSettingsDomainModelHandler(ICampaignSettingsRepository campaignSettingsRepository) =>
            _campaignSettingsRepository = campaignSettingsRepository ?? throw new ArgumentNullException(nameof(campaignSettingsRepository));

        public CampaignSettings Add(CampaignSettings model)
        {
            _campaignSettingsRepository.Add(model);
            return model;
        }

        public void AddRange(params CampaignSettings[] models) => _campaignSettingsRepository.AddRange(models);

        public int Count() => _campaignSettingsRepository.Count();

        public void DeleteAll() => _campaignSettingsRepository.Truncate();

        public IEnumerable<CampaignSettings> GetAll() => _campaignSettingsRepository.GetAll();
    }
}
