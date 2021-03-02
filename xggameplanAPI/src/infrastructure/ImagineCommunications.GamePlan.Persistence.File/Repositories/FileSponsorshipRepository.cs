using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Sponsorships;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using xgcore.auditevents.Repository.File;

namespace ImagineCommunications.GamePlan.Persistence.File.Repositories
{
    public class FileSponsorshipRepository
        : FileRepositoryBase, ISponsorshipRepository
    {
        public FileSponsorshipRepository(string folder) : base(folder, "sponsorship")
        { }

        public Sponsorship Get(string externalReferenceId) =>
            GetItemByID<Sponsorship>(_folder, _type, externalReferenceId);

        public IEnumerable<Sponsorship> GetAll() =>
            GetAllByType<Sponsorship>(_folder, _type);

        public void Add(Sponsorship item)
        {
            var items = new List<Sponsorship>() { item };
            InsertItems(_folder, _type, items, items.Select(i => i.ExternalReferenceId).ToList());
        }

        public void Update(Sponsorship item) =>
            UpdateOrInsertItem(_folder, _type, item, item.ExternalReferenceId);

        public void Delete(string externalReferenceId) =>
            DeleteItem<Sponsorship>(_folder, _type, externalReferenceId);

        public void SaveChanges() { }

        public bool Exists(string externalReferenceId) =>
            GetItemByID<Sponsorship>(_folder, _type, externalReferenceId) != null;

        public void Truncate()
        {
            DeleteAllItems<Sponsorship>(_folder, _type);
        }

        public Task TruncateAsync()
        {
            Truncate();
            return Task.FromResult(true);
        }
    }
}
