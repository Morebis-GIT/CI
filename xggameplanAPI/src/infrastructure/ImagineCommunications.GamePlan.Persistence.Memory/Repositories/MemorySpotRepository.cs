using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Spots;

namespace ImagineCommunications.GamePlan.Persistence.Memory.Repositories
{
    public class MemorySpotRepository :
        MemoryRepositoryBase<Spot>,
        ISpotRepository
    {
        public MemorySpotRepository()
        {
        }

        public void Add(IEnumerable<Spot> items)
        {
            var spots = items.ToList();
            InsertItems(spots, spots.Select(i => i.Uid.ToString()).ToList());
        }

        public void Add(Spot item)
        {
            var items = new List<Spot>() { item };
            InsertItems(items, items.Select(i => i.Uid.ToString()).ToList());
        }

        public void Update(Spot item)
        {
            UpdateOrInsertItem(item, item.Uid.ToString());
        }

        public IEnumerable<Spot> FindByExternal(string campref)
        {
            return GetAllItems(s => s.ExternalCampaignNumber == campref);
        }

        public IEnumerable<Spot> FindByExternalBreakNumbers(IEnumerable<string> externalBreakNumbers)
        {
            return GetAllItems(s => externalBreakNumbers.Contains(s.ExternalBreakNo));
        }

        public Spot FindByExternalSpotRef(string externalSpotRef)
        {
            return GetAllItems(s => s.ExternalSpotRef == externalSpotRef).FirstOrDefault();
        }

        public IEnumerable<Spot> Search(DateTime datefrom, DateTime dateto, string salesarea)
        {
            var items = GetAllItems(currentItem => currentItem.SalesArea == salesarea &&
                                                     currentItem.StartDateTime >= datefrom &&
                                                     currentItem.StartDateTime <= dateto);

            return items.ToList();
        }

        public IEnumerable<Spot> Search(DateTime datefrom, DateTime dateto, List<string> salesareas) =>
            GetAllItems(s =>
                s.StartDateTime >= datefrom
                && s.StartDateTime <= dateto
                && salesareas.Contains(s.SalesArea));

        public Spot Find(Guid id) => GetItemById(id.ToString());

        public IEnumerable<Spot> GetAll() => GetAllItems();

        public IEnumerable<Spot> GetAllMultipart() => GetAllItems(s => s.IsMultipartSpot);

        public IEnumerable<Spot> GetAllByCampaign(string campaignExternalId) =>
            GetAllItems(s => s.ExternalCampaignNumber == campaignExternalId);

        public int CountAll => GetCount();

        public void Remove(Guid uid)
        {
            DeleteItem(uid.ToString());
        }

        public void Delete(IEnumerable<Guid> ids)
        {
            DeleteAllItems(b => ids.Contains(b.Uid));
        }

        public void Truncate()
        {
            DeleteAllItems();
        }

        public Task TruncateAsync()
        {
            Truncate();
            return Task.FromResult(true);
        }

        public IEnumerable<Spot> FindByExternal(List<string> externalRef) =>
            GetAllItems(s => externalRef.Contains(s.ExternalSpotRef));

        public void SaveChanges() { }

        public int Count(Expression<Func<Spot, bool>> query) => GetCount(query);

        public decimal GetNominalPriceByCampaign(string campaignExternalId) =>
            GetAllItems(s => s.ExternalCampaignNumber == campaignExternalId && s.ClientPicked == false && s.IsUnplaced == false)
                .Sum(s => s.NominalPrice);

        public void Dispose() { }

        public void InsertOrReplace(IEnumerable<Spot> items) => throw new NotImplementedException();
    }
}
