using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;

namespace ImagineCommunications.GamePlan.Persistence.Memory.Repositories
{
    public class MemoryCampaignRepository :
        MemoryRepositoryBase<Campaign>,
        ICampaignRepository
    {
        public MemoryCampaignRepository()
        {
        }

        public void Dispose()
        {
        }

        public void Add(IEnumerable<Campaign> items)
        {
            foreach (var item in items)
            {
                _ = Add(item);
            }
        }

        public Campaign Add(Campaign item)
        {
            item.UpdateDerivedKPIs();

            if (item.CustomId == 0)
            {
                item.CustomId = FindCustomId();
            }

            InsertOrReplaceItem(item, item.Id.ToString());

            return item;
        }

        private int FindCustomId()
        {
            var idlist = GetAllItems().Select(c => c.CustomId).ToArray<int>();

            return idlist?.Length > 0 ? idlist.Max() + 33 : 1;
        }

        public Campaign Find(Guid id) => Get(id);

        public Campaign Get(Guid id) => GetItemById(id.ToString());

        public IEnumerable<Campaign> GetByGroup(string group) => throw new NotImplementedException();

        public IEnumerable<string> GetAllActiveExternalIds() => throw new NotImplementedException();

        public IEnumerable<Campaign> Find(List<Guid> uids) => GetAllItems(c => uids.Contains(c.Id));

        public IEnumerable<Campaign> FindByRef(string externalref) => GetAllItems(c => c.ExternalId == externalref);

        public IEnumerable<Campaign> FindByRefs(List<string> externalRefs) => GetAllItems(c => externalRefs.Contains(c.ExternalId));

        public IEnumerable<Campaign> FindMissingCampaignsFromGroup(
            List<string> externalRefs,
            List<string> campaignGroup
        ) => throw new NotImplementedException();

        public IEnumerable<Campaign> GetAll() => GetAllItems();

        public IEnumerable<CampaignReducedModel> GetAllFlat() => throw new NotImplementedException();

        public PagedQueryResult<CampaignWithProductFlatModel> GetWithProduct(CampaignSearchQueryModel queryModel)
        {
            throw new NotImplementedException();
        }

        public int CountAll => GetCount();

        public int CountAllActive => GetAllActive().Count();

        public IEnumerable<Campaign> GetAllScenarioUI()
        {
            return GetAllItems(c => c.SalesAreaCampaignTarget != null &&
                                    c.SalesAreaCampaignTarget.Count > 0 &&
                                    !c.Status.Equals("C", StringComparison.OrdinalIgnoreCase)).Where(c => c.ActualRatings >= c.TargetRatings).ToList();
        }

        public IEnumerable<Campaign> GetAllActive()
        {
            return GetAllItems(c =>
                (c.DeliveryType == CampaignDeliveryType.Spot ? c.TargetRatings >= default(decimal) : c.TargetZeroRatedBreaks || c.TargetRatings > default(decimal)) &&
                c.SalesAreaCampaignTarget != null &&
                c.SalesAreaCampaignTarget.Count > 0 &&
                !c.Status.Equals("C", StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public IEnumerable<CampaignNameModel> FindNameByRefs(ICollection<string> externalRefs) => throw new NotImplementedException();

        public IEnumerable<string> GetBusinessTypes() => throw new NotImplementedException();

        public void Remove(Guid uid) => Delete(uid);

        public void Delete(Guid uid)
        {
            DeleteItem(uid.ToString());
        }

        public void Delete(IEnumerable<string> campaignExternalIds) => throw new NotImplementedException();

        public void Truncate()
        {
            DeleteAllItems();
        }

        public Task TruncateAsync()
        {
            Truncate();
            return Task.CompletedTask;
        }

        public void SetCustomIds() => throw new NotImplementedException();

        public void SaveChanges()
        {
        }

        public void Update(Campaign campaign)
        {
            campaign.UpdateDerivedKPIs();

            if (campaign.CustomId == 0)
            {
                campaign.CustomId = FindCustomId();
            }

            InsertOrReplaceItem(campaign, campaign.Id.ToString());
        }
    }
}
