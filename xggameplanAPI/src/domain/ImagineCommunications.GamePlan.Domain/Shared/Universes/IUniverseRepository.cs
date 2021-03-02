using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Shared.Universes
{
    public interface IUniverseRepository
    {
        Universe Find(Guid id);

        /// <summary>
        /// Get all universes
        /// </summary>
        /// <returns></returns>
        IEnumerable<Universe> GetAll();

        IEnumerable<Universe> GetBySalesAreaDemo(string salesarea, string demographic);

        IEnumerable<Universe> Search(List<string> demographics, List<string> salesAreas, DateTime startDate,
            DateTime endDate);

        void Insert(IEnumerable<Universe> universes);

        void Update(Universe universe);

        void Remove(Guid id);

        void Truncate();

        void DeleteByCombination(string salesArea, string demographic, DateTime? startDate, DateTime? endDate);
        void SaveChanges();
    }
}
