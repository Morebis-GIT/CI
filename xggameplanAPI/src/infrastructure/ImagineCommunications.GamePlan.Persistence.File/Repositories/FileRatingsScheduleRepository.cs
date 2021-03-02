using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using xgcore.auditevents.Repository.File;

namespace ImagineCommunications.GamePlan.Persistence.File.Repositories
{
    public class FileRatingsScheduleRepository
        : FileRepositoryBase, IRatingsScheduleRepository
    {
        public FileRatingsScheduleRepository(string folder)
            : base(folder, "ratings_schedules")
        {

        }

        public void Dispose()
        {

        }

        public IEnumerable<RatingsPredictionSchedule> GetAll()
        {
            return GetAllByType<RatingsPredictionSchedule>(_folder, _type);
        }

        public int CountAll
        {
            get
            {
                return CountAll<RatingsPredictionSchedule>(_folder, _type);
            }
        }

        private string GetID(string salesArea, DateTime scheduleDate)
        {
            return string.Format("{0}.{1}", salesArea, scheduleDate.ToString("yyyy-MM-dd"));
        }

        /// <summary>
        /// Add new entry in schedule
        /// </summary>
        /// <param name="item"></param>
        public void Add(RatingsPredictionSchedule item)
        {
            List<RatingsPredictionSchedule> items = new List<RatingsPredictionSchedule>() { item };
            InsertItems(_folder, _type, items, items.Select(i => GetID(i.SalesArea, i.ScheduleDay)).ToList());
        }

        public void Update(RatingsPredictionSchedule item)
        {
            UpdateOrInsertItem(_folder, _type, item, GetID(item.SalesArea, item.ScheduleDay));
        }

        public void Insert(List<RatingsPredictionSchedule> items, bool setIdentity = true)
        {
            InsertItems(_folder, _type, items.ToList(), items.Select(i => GetID(i.SalesArea, i.ScheduleDay)).ToList());
        }

        public void Remove(RatingsPredictionSchedule item)
        {
            DeleteItem<RatingsPredictionSchedule>(_folder, _type, GetID(item.SalesArea, item.ScheduleDay));
        }

        public void DeleteRange(IEnumerable<int> ids)
        {
            DeleteAllItems<RatingsPredictionSchedule>(_folder, _type, b => ids.Contains(b.Id));
        }

        /// <summary>
        /// GetSchedule by salesarea and scheduledate
        /// </summary>
        /// <param name="salesarea"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public RatingsPredictionSchedule GetSchedule(DateTime date, string salesarea)
        {
            return GetAllByType<RatingsPredictionSchedule>(_folder, _type, s => s.SalesArea == salesarea && s.ScheduleDay == date).FirstOrDefault();
        }

        /// <summary>
        /// Gets schedules for date range and sales area
        /// </summary>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="salesarea"></param>
        /// <returns></returns>
        public List<RatingsPredictionSchedule> GetSchedules(DateTime fromDateTime, DateTime toDateTime, string salesarea)
        {
            var items = GetAllByType<RatingsPredictionSchedule>(_folder, _type, p => p.ScheduleDay <= toDateTime.Date && p.ScheduleDay >= fromDateTime.Date && p.SalesArea == salesarea);
            return items;
        }

        public void Truncate()
        {
            DeleteAllItems<RatingsPredictionSchedule>(_folder, _type);
        }

        public Task TruncateAsync()
        {
            Truncate();
            return Task.FromResult(true);
        }

        public void SaveChanges() { }

        public List<RatingsPredictionValidationMessage> Validate_RatingsPredictionSchedules(
            DateTime fromDateTime,
            DateTime toDateTime,
            IEnumerable<string> salesAreaNames,
            IEnumerable<string> demographics,
            int noOfRatingPredictionsPerScheduleDayAreaDemo) => throw new NotImplementedException();
    }
}
