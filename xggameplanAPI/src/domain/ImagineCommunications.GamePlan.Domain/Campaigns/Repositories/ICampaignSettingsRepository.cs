using System.Collections.Generic;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;

namespace ImagineCommunications.GamePlan.Domain.Campaigns
{
    public interface ICampaignSettingsRepository
    {
        void Add(CampaignSettings item);

        void AddRange(IEnumerable<CampaignSettings> items);

        void Update(CampaignSettings item);

        void Delete(int id);

        void DeleteByExternal(string externalId);

        CampaignSettings Get(int id);

        CampaignSettings GetByExternalId(string externalId);

        IEnumerable<CampaignSettings> GetAll();

        void SaveChanges();

        int Count();

        bool Exists(string externalId);

        void Truncate();

        Task TruncateAsync();
    }
}
