using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage.Objects;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage
{
    public interface IAutoBookRepository
    {
        void Add(AutoBook autoBook);

        AutoBook Get(string id);

        void Delete(string id);

        IEnumerable<AutoBook> GetAll();

        int CountAll { get; }

        void SaveChanges();

        void Update(AutoBook autoBook);
    }
}
