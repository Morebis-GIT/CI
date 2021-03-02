using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class CampaignDomainModelHandler : IDomainModelHandler<Campaign>
    {
        private readonly ICampaignRepository _campaignRepository;

        public CampaignDomainModelHandler(ICampaignRepository campaignRepository) =>
            _campaignRepository = campaignRepository ?? throw new ArgumentNullException(nameof(campaignRepository));

        public Campaign Add(Campaign model)
        {
            _ = _campaignRepository.Add(model);
            return model;
        }

        public void AddRange(params Campaign[] models) => _campaignRepository.Add(models);

        public int Count() => _campaignRepository.CountAll;

        public void DeleteAll() => _campaignRepository.Truncate();

        public IEnumerable<Campaign> GetAll() => _campaignRepository.GetAll();
    }
}
