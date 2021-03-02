using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.TotalRatings
{
    public interface ITotalRatingRepository
    {
        IEnumerable<TotalRating> GetAll();
        TotalRating Get(int id);
        void AddRange(IEnumerable<TotalRating> totalRatings);
        IEnumerable<TotalRating> Search(string salesArea, DateTime startDate, DateTime endDate);
        IEnumerable<TotalRating> SearchByMonths(DateTime startDate, DateTime endDate);
        void DeleteRange(IEnumerable<int> ids);
        void SaveChanges();
        void Truncate();
    }
}
