using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Spots;
using xgcore.auditevents.Repository.File;

namespace ImagineCommunications.GamePlan.Persistence.File.Repositories
{
    public class FileSpotRepository
        : FileRepositoryBase, ISpotRepository
    {
        public FileSpotRepository(string folder) : base(folder, "spot") { }

        public void Add(IEnumerable<Spot> items)
        {
            InsertItems(
                _folder,
                _type,
                items.ToList(),
                items.Select(i => i.Uid.ToString()).ToList());
        }

        public void Add(Spot item)
        {
            List<Spot> items = new List<Spot>() { item };
            InsertItems(_folder, _type, items, items.Select(i => i.Uid.ToString()).ToList());
        }

        public void Update(Spot item)
        {
            UpdateOrInsertItem(_folder, _type, item, item.Uid.ToString());
        }

        public IEnumerable<Spot> FindByExternal(string campref)
        {
            return GetAllByType<Spot>(_folder, _type, s => s.ExternalCampaignNumber == campref);
        }

        public Spot FindByExternalSpotRef(string externalSpotRef)
        {
            return GetAllByType<Spot>(_folder, _type, s => s.ExternalSpotRef == externalSpotRef).FirstOrDefault();
        }

        public IEnumerable<Spot> Search(DateTime datefrom, DateTime dateto, string salesarea)
        {
            var items = GetAllByType<Spot>(_folder, _type, currentItem => currentItem.SalesArea == salesarea &&
                                                     currentItem.StartDateTime >= datefrom &&
                                                     currentItem.StartDateTime <= dateto);
            return items.ToList();
        }

        public IEnumerable<Spot> Search(DateTime datefrom, DateTime dateto, List<string> salesareas)
        {
            var items = GetAllByType<Spot>(_folder, _type, s =>
                s.StartDateTime >= datefrom && s.StartDateTime <= dateto && salesareas.Contains(s.SalesArea));

            return items;
        }

        public Spot Find(Guid id)
        {
            return GetItemByID<Spot>(_folder, _type, id.ToString());
        }

        public IEnumerable<Spot> GetAll()
        {
            return GetAllByType<Spot>(_folder, _type);
        }

        public IEnumerable<Spot> GetAllMultipart() => GetAllByType<Spot>(_folder, _type, s => s.IsMultipartSpot);

        public IEnumerable<Spot> GetAllByCampaign(string campaignExternalId) =>
            GetAllByType<Spot>(_folder, _type, s => s.ExternalCampaignNumber == campaignExternalId);

        public int CountAll
        {
            get
            {
                return CountAll<Spot>(_folder, _type);
            }
        }

        public void Remove(Guid uid)
        {
            DeleteItem<Spot>(_folder, _type, uid.ToString());
        }

        public void Delete(IEnumerable<Guid> ids)
        {
            DeleteAllItems<Spot>(_folder, _type, b => ids.Contains(b.Uid));
        }

        public void Truncate()
        {
            DeleteAllItems<Spot>(_folder, _type);
        }

        public Task TruncateAsync()
        {
            Truncate();
            return Task.FromResult(true);
        }

        public IEnumerable<Spot> FindByExternal(List<string> externalRef)
        {
            return GetAllByType<Spot>(_folder, _type, s => externalRef.Contains(s.ExternalSpotRef));
        }

        public IEnumerable<Spot> FindByExternalBreakNumbers(IEnumerable<string> externalBreakNumbers)
        {
            return GetAllByType<Spot>(_folder, _type, s => externalBreakNumbers.Contains(s.ExternalBreakNo));
        }

        public void SaveChanges()
        {

        }

        public void Dispose()
        {

        }

        public int Count(Expression<Func<Spot, bool>> query)
        {
            return GetAllByType(_folder, _type, query).Count;
        }

        public decimal GetNominalPriceByCampaign(string campaignExternalId) =>
            GetAllByType<Spot>(_folder, _type, s => s.ExternalCampaignNumber == campaignExternalId && s.ClientPicked == false && s.IsUnplaced == false)
                .Sum(s => s.NominalPrice);

        public void InsertOrReplace(IEnumerable<Spot> items) => throw new NotImplementedException();
    }
}
