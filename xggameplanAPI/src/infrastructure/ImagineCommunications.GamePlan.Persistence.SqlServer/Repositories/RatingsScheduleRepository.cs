using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.PostProcessing.Builders;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.PredictionSchedules;
using Microsoft.EntityFrameworkCore;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class RatingsScheduleRepository : IRatingsScheduleRepository
    {
        private readonly ISqlServerDbContext _dbContext;
        private readonly IMapper _mapper;

        public RatingsScheduleRepository(ISqlServerDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public int CountAll => _dbContext.Query<PredictionSchedule>().Count();

        public RatingsPredictionSchedule GetSchedule(DateTime fromDateTime, string salesarea) =>
            _dbContext.Query<PredictionSchedule>()
                .ProjectTo<RatingsPredictionSchedule>(_mapper.ConfigurationProvider)
                .FirstOrDefault(x => x.SalesArea == salesarea && x.ScheduleDay == fromDateTime);

        public List<RatingsPredictionSchedule> GetSchedules(DateTime fromDateTime, DateTime toDateTime, string salesarea) =>
            _dbContext.Query<PredictionSchedule>()
                .Where(x => x.ScheduleDay <= toDateTime.Date && x.ScheduleDay >= fromDateTime.Date && x.SalesArea == salesarea)
                .ProjectTo<RatingsPredictionSchedule>(_mapper.ConfigurationProvider)
                .ToList();

        public IEnumerable<RatingsPredictionSchedule> GetAll() =>
            _dbContext.Query<PredictionSchedule>().ProjectTo<RatingsPredictionSchedule>(_mapper.ConfigurationProvider).ToList();

        public void Add(RatingsPredictionSchedule item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var entity = _dbContext.Find<PredictionSchedule>(new object[] { item.Id },
                find => find.IncludeCollection(pb => pb.Ratings));
            if (entity == null)
            {
                _dbContext.Add(_mapper.Map<PredictionSchedule>(item), post => post.MapTo(item), _mapper);
            }
            else
            {
                _mapper.Map(item, entity);
                _dbContext.Update(entity, post => post.MapTo(item), _mapper);
            }
        }

        public void Insert(List<RatingsPredictionSchedule> items, bool setIdentity = true)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (!items.Any())
            {
                return;
            }

            var entities = _mapper.Map<List<PredictionSchedule>>(items);
            using var transaction = _dbContext.Specific.Database.BeginTransaction();

            _dbContext.BulkInsertEngine.BulkInsert(entities,
                new BulkInsertOptions { PreserveInsertOrder = true, SetOutputIdentity = true });

            var ratingEntities = entities.SelectMany(x => x.Ratings.Select(
                r =>
                {
                    r.PredictionScheduleId = x.Id;
                    return r;
                })).ToList();

            var ratingBulkInsertOptions = setIdentity
                ? new BulkInsertOptions { PreserveInsertOrder = true, SetOutputIdentity = true }
                : new BulkInsertOptions { BatchSize = 200000 };

            _dbContext.BulkInsertEngine.BulkInsert(ratingEntities, ratingBulkInsertOptions);
            transaction.Commit();

            if (setIdentity)
            {
                var actionBuilder = new BulkInsertActionBuilder<PredictionSchedule>(entities, _mapper);
                actionBuilder.TryToUpdate(items);
                actionBuilder.Build()?.Execute();
            }
        }

        public void Remove(RatingsPredictionSchedule item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var entity = _dbContext.Find<PredictionSchedule>(item.Id);
            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public void DeleteRange(IEnumerable<int> ids)
        {
            var predictionSchedules = _dbContext.Query<PredictionSchedule>()
                .Where(x => ids.Contains(x.Id))
                .ToArray();

            if (predictionSchedules.Any())
            {
                _dbContext.RemoveRange(predictionSchedules);
            }
        }

        public List<RatingsPredictionValidationMessage> Validate_RatingsPredictionSchedules(DateTime fromDateTime, DateTime toDateTime,
            IEnumerable<string> salesAreaNames, IEnumerable<string> demographics, int noOfRatingPredictionsPerScheduleDayAreaDemo)
        {
            var spanOfDays = toDateTime.Date - fromDateTime.Date;
            var noOfDaysInRun = spanOfDays.Days + 1; //inclusive of fromDate

            var messages = new List<RatingsPredictionValidationMessage>();

            var allRatingsScheduleForDemographicsEnabled = _dbContext.Query<PredictionScheduleRating>()
                .Include(x => x.PredictionSchedule)
                .Where(r => r.PredictionSchedule.ScheduleDay >= fromDateTime.Date &&
                    r.PredictionSchedule.ScheduleDay <= toDateTime.Date.AddDays(1) &&
                    salesAreaNames.Contains(r.PredictionSchedule.SalesArea) &&
                    demographics.Contains(r.Demographic))
                .GroupBy(r => new { r.PredictionSchedule.SalesArea, r.PredictionSchedule.ScheduleDay, r.Demographic })
                .Select(g => new
                {
                    g.Key.SalesArea,
                    g.Key.ScheduleDay,
                    g.Key.Demographic,
                    Count = g.Count()
                }).ToArray();

            //check that we have 96 (4 x 24) ratings for each day for each sales area, report where we dont
            //(this could be less than 96 where individual ratings are missing from a document or more than 96 say 192 for duplicate documents)
            var incorrectRatings = allRatingsScheduleForDemographicsEnabled
                .Where(r => r.Count != noOfRatingPredictionsPerScheduleDayAreaDemo).ToList();
            foreach (var item in incorrectRatings)
            {
                messages.Add(new RatingsPredictionValidationMessage { Message = $"Count: {item.Count} for Demographic: {item.Demographic}, incorrect in Sales Area: " +
                    $"{item.SalesArea}, in Schedule Day: {item.ScheduleDay.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}"});
            }

            // check for missing whole documents by date and sales area from the above in memory collection
            var missingRatingPredictionDays = new List<string>();
            foreach (string salesAreaName in salesAreaNames)
            {
                for (int i = 0; i < noOfDaysInRun; i++)
                {
                    var ratingsScheduleForThisDay = allRatingsScheduleForDemographicsEnabled.Where(r =>
                        r.SalesArea == salesAreaName
                        && r.ScheduleDay == fromDateTime.Date.AddDays(i)).ToList();

                    if (ratingsScheduleForThisDay.Count == 0)
                    {
                        missingRatingPredictionDays.Add(fromDateTime.Date.AddDays(i).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                    }
                }

                if (missingRatingPredictionDays.Any())
                {
                    messages.Add(new RatingsPredictionValidationMessage
                    {
                        SeverityLevel = ValidationSeverityLevel.Warning,
                        Message = $"Missing Schedule Days: {string.Join(", ", missingRatingPredictionDays)} for the following Sales Area: {salesAreaName}"
                    });

                    missingRatingPredictionDays.Clear();
                }
            }

            return messages;
        }

        public async Task TruncateAsync()
        {
            await _dbContext.TruncateAsync<PredictionSchedule>().ConfigureAwait(false);
        }

        public void SaveChanges() => _dbContext.SaveChanges();
    }
}
