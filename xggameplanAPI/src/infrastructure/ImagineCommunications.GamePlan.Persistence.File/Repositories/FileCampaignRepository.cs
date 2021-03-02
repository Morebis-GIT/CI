using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using xgcore.auditevents.Repository.File;

namespace ImagineCommunications.GamePlan.Persistence.File.Repositories
{
    public class FileCampaignRepository
        : FileRepositoryBase, ICampaignRepository
    {
        public FileCampaignRepository(string folder) : base(folder, "campaign")
        {
        }

        public void Dispose()
        {
        }

        public void Add(IEnumerable<Campaign> items)
        {
            foreach (var campaign in items)
            {
                campaign.UpdateDerivedKPIs();
            }

            // NOTE: Raven repo doesn't set CustomId
            InsertItems(_folder, _type, items.ToList(), items.Select(i => i.Id.ToString()).ToList());
        }

        public Campaign Add(Campaign item)
        {
            item.UpdateDerivedKPIs();

            List<Campaign> items = new List<Campaign>() { item };
            if (item.CustomId == 0)
            {
                item.CustomId = FindCustomId();
            }

            InsertItems(_folder, _type, items, items.ConvertAll(i => i.Id.ToString()));
            return item;
        }

        private int FindCustomId()
        {
            var idlist = GetAllByType<Campaign>(_folder, _type).Select(c => c.CustomId).ToArray();
            return idlist?.Length > 0 ? idlist.Max() + 33 : 1;
        }

        public Campaign Find(Guid id) => Get(id);

        public Campaign Get(Guid id)
        {
            return GetItemByID<Campaign>(_folder, _type, id.ToString());
        }

        public IEnumerable<Campaign> Find(List<Guid> uids)
        {
            return GetAllByType<Campaign>(_folder, _type, c => uids.Contains(c.Id));
        }

        public IEnumerable<Campaign> FindByRef(string externalref)
        {
            return GetAllByType<Campaign>(_folder, _type, c => c.ExternalId == externalref);
        }

        public IEnumerable<Campaign> FindByRefs(List<string> externalRefs)
        {
            return GetAllByType<Campaign>(_folder, _type, c => externalRefs.Contains(c.ExternalId));
        }

        public IEnumerable<Campaign> GetAll()
        {
            return GetAllByType<Campaign>(_folder, _type);
        }

        public IEnumerable<CampaignReducedModel> GetAllFlat() => throw new NotImplementedException();

        public PagedQueryResult<CampaignWithProductFlatModel> GetWithProduct(CampaignSearchQueryModel queryModel)
        {
            throw new NotImplementedException();
        }

        public int CountAll => CountAll(_folder, _type);

        public int CountAllActive => GetAllActive().Count();

        public IEnumerable<Campaign> GetAllScenarioUI()
        {
            return GetAllByType<Campaign>(_folder, _type, c => c.SalesAreaCampaignTarget != null &&
                                    c.SalesAreaCampaignTarget.Count > 0 &&
                                    !c.Status.Equals("C", StringComparison.OrdinalIgnoreCase)).Where(c => c.ActualRatings >= c.TargetRatings).ToList();
        }

        public IEnumerable<Campaign> GetAllActive()
        {
            return GetAllByType<Campaign>(_folder, _type, c =>
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
            DeleteItem(_folder, _type, uid.ToString());
        }

        public void Truncate()
        {
            DeleteAllItems(_folder, _type);
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

        public IEnumerable<Campaign> GetByGroup(string group) => throw new NotImplementedException();

        public IEnumerable<Campaign> FindMissingCampaignsFromGroup(
            List<string> externalRefs,
            List<string> campaignGroup
        ) => throw new NotImplementedException();

        public void Update(Campaign campaign)
        {
            campaign.UpdateDerivedKPIs();

            if (campaign.CustomId == 0)
            {
                campaign.CustomId = FindCustomId();
            }

            UpdateOrInsertItem(_folder, _type, campaign, campaign.Id.ToString());
        }

        public IEnumerable<string> GetAllActiveExternalIds() => throw new NotImplementedException();

        public void Delete(IEnumerable<string> campaignExternalIds) => throw new NotImplementedException();
    }
}
