using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class CampaignSettingsRepositoryAdapter : RepositoryTestAdapter<CampaignSettings, ICampaignSettingsRepository, int>
    {
        public CampaignSettingsRepositoryAdapter(IScenarioDbContext dbContext, ICampaignSettingsRepository repository) : base(dbContext, repository)
        {
        }

        protected override CampaignSettings Add(CampaignSettings model)
        {
            Repository.Add(model);
            return model;
        }

        protected override CampaignSettings Update(CampaignSettings model)
        {
            Repository.Update(model);
            return model;
        }

        protected override void Delete(int id) => Repository.Delete(id);

        protected override IEnumerable<CampaignSettings> GetAll() => Repository.GetAll();

        protected override CampaignSettings GetById(int id) => Repository.Get(id);

        protected override IEnumerable<CampaignSettings> AddRange(params CampaignSettings[] models)
        {
            Repository.AddRange(models);
            return models;
        }

        protected override int Count() => Repository.Count();

        protected override void Truncate() => Repository.TruncateAsync().Wait();
    }
}
