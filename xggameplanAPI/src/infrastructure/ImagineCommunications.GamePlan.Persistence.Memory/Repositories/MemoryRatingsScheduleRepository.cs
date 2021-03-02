using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;

namespace ImagineCommunications.GamePlan.Persistence.Memory.Repositories
{
    public class MemoryRatingsScheduleRepository :
        MemoryRepositoryBase<RatingsPredictionSchedule>,
        IRatingsScheduleRepository
    {
        public MemoryRatingsScheduleRepository()
        {
        }

        public void Dispose()
        {
        }

        public IEnumerable<RatingsPredictionSchedule> GetAll() => GetAllItems();

        public int CountAll => GetCount();

        private string GetID(string salesArea, DateTime scheduleDate) =>
            $"{salesArea}.{scheduleDate.ToString("yyyy-MM-dd")}";

        /// <summary>
        /// Add new entry in schedule
        /// </summary>
        /// <param name="item"></param>
        public void Add(RatingsPredictionSchedule item)
        {
            var items = new List<RatingsPredictionSchedule>() { item };
            InsertItems(items, items.Select(i => GetID(i.SalesArea, i.ScheduleDay)).ToList());
        }

        public void Update(RatingsPredictionSchedule item)
        {
            UpdateOrInsertItem(item, GetID(item.SalesArea, item.ScheduleDay));
        }

        public void Insert(List<RatingsPredictionSchedule> items, bool setIdentity = true)
        {
            InsertItems(items.ToList(), items.Select(i => GetID(i.SalesArea, i.ScheduleDay)).ToList());
        }

        public void Remove(RatingsPredictionSchedule item)
        {
            DeleteItem(GetID(item.SalesArea, item.ScheduleDay));
        }

        public void DeleteRange(IEnumerable<int> ids)
        {
            DeleteAllItems(b => ids.Contains(b.Id));
        }

        /// <summary>
        /// GetSchedule by salesarea and scheduledate
        /// </summary>
        /// <param name="date"></param>
        /// <param name="salesarea"></param>
        /// <returns></returns>
        public RatingsPredictionSchedule GetSchedule(DateTime date, string salesarea) =>
            GetAllItems(s => s.SalesArea == salesarea && s.ScheduleDay == date).FirstOrDefault();

        /// <summary>
        /// Gets schedules for date range and sales area
        /// </summary>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="salesarea"></param>
        /// <returns></returns>
        public List<RatingsPredictionSchedule> GetSchedules(DateTime fromDateTime, DateTime toDateTime, string salesarea) =>
            GetAllItems(p => p.ScheduleDay <= toDateTime.Date && p.ScheduleDay >= fromDateTime.Date && p.SalesArea == salesarea);

        public void Truncate()
        {
            DeleteAllItems();
        }

        public Task TruncateAsync()
        {
            Truncate();
            return Task.FromResult(true);
        }

        public void SaveChanges() { }

        /// <summary>
        /// Checks the Demographic/SalesArea/ScheduleDay where there are not
        /// ratings prediction for all 96 slots in a day (4 x 24). Checks that
        /// there are the expected number of documents for each date and sales
        /// area in the run.
        /// </summary>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="salesAreaNames"></param>
        /// <param name="demographicsNames"></param>
        /// <param name="noOfRatingPredictionsPerScheduleDayAreaDemo"></param>
        /// <returns>List of messages for any errors in the document collection</returns>
        public List<RatingsPredictionValidationMessage> Validate_RatingsPredictionSchedules(
            DateTime fromDateTime,
            DateTime toDateTime,
            IEnumerable<string> salesAreaNames,
            IEnumerable<string> demographics,
            int noOfRatingPredictionsPerScheduleDayAreaDemo) => throw new NotImplementedException();
    }
}
