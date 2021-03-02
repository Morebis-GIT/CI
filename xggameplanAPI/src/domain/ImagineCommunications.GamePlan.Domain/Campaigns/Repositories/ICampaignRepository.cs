using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;

namespace ImagineCommunications.GamePlan.Domain.Campaigns
{
    public interface ICampaignRepository
    {
        Campaign Add(Campaign item);

        void Add(IEnumerable<Campaign> item);

        IEnumerable<Campaign> GetAll();

        IEnumerable<CampaignReducedModel> GetAllFlat();

        IEnumerable<string> GetAllActiveExternalIds();

        PagedQueryResult<CampaignWithProductFlatModel> GetWithProduct(CampaignSearchQueryModel queryModel);

        int CountAll { get; }

        int CountAllActive { get; }

        IEnumerable<Campaign> GetAllActive();

        IEnumerable<Campaign> GetAllScenarioUI();

        [Obsolete("Use the Get() method")]
        Campaign Find(Guid uid);

#pragma warning disable CA1716 // Identifiers should not match keywords
        Campaign Get(Guid uid);
#pragma warning restore CA1716 // Identifiers should not match keywords

        IEnumerable<Campaign> Find(List<Guid> uids);

        IEnumerable<Campaign> FindByRef(string externalref);

        IEnumerable<Campaign> GetByGroup(string group);

        IEnumerable<Campaign> FindByRefs(List<string> externalRefs);

        IEnumerable<Campaign> FindMissingCampaignsFromGroup(List<string> externalRefs, List<string> campaignGroup);

        IEnumerable<CampaignNameModel> FindNameByRefs(ICollection<string> externalRefs);

        IEnumerable<string> GetBusinessTypes();

        [Obsolete("Use the Delete() method")]
        void Remove(Guid uid);

        void Delete(Guid uid);

        void Delete(IEnumerable<string> campaignExternalIds);

        void Truncate();

        Task TruncateAsync();

        void Update(Campaign campaign);

        void SaveChanges();
    }
}
