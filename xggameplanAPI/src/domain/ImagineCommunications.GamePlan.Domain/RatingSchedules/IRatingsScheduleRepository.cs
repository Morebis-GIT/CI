using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImagineCommunications.GamePlan.Domain.RatingSchedules
{
    public interface IRatingsScheduleRepository
    {
        int CountAll { get; }

        RatingsPredictionSchedule GetSchedule(DateTime fromDateTime, string salesarea);

        List<RatingsPredictionSchedule> GetSchedules(DateTime fromDateTime, DateTime toDateTime, string salesarea);

        IEnumerable<RatingsPredictionSchedule> GetAll();

        void Add(RatingsPredictionSchedule item);

        void Insert(List<RatingsPredictionSchedule> items, bool setIdentity = true);

        void Remove(RatingsPredictionSchedule item);

        void DeleteRange(IEnumerable<int> ids);

        List<RatingsPredictionValidationMessage> Validate_RatingsPredictionSchedules(DateTime fromDateTime, DateTime toDateTime, 
            IEnumerable<string> salesAreaNames, IEnumerable<string> demographics, int noOfRatingPredictionsPerScheduleDayAreaDemo);

        Task TruncateAsync();

        void SaveChanges();
    }
}
