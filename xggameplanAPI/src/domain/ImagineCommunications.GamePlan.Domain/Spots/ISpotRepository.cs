using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;

namespace ImagineCommunications.GamePlan.Domain.Spots
{
    public interface ISpotRepository : IRepository<Spot>
    {
        void Update(Spot item);

        IEnumerable<Spot> Search(DateTime datefrom, DateTime dateto, string salesarea);

        IEnumerable<Spot> Search(DateTime datefrom, DateTime dateto, List<string> salesareas);

        IEnumerable<Spot> FindByExternalBreakNumbers(IEnumerable<string> externalBreakNumbers);

        Spot FindByExternalSpotRef(string externalSpotRef);

        IEnumerable<Spot> GetAllMultipart();

        IEnumerable<Spot> GetAllByCampaign(string campaignExternalId);

        decimal GetNominalPriceByCampaign(string campaignExternalId);

        void InsertOrReplace(IEnumerable<Spot> items);

        void Delete(IEnumerable<Guid> ids);

        void SaveChanges();

        Task TruncateAsync();
    }
}
