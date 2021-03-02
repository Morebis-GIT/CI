using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.PostProcessing.Builders;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.PredictionSchedules;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using Microsoft.EntityFrameworkCore;
using xggameplan.core.Extensions.AutoMapper;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class RatingsScheduleRepository : IRatingsScheduleRepository
    {
        private readonly ISqlServerDbContext _dbContext;
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;
        private readonly IMapper _mapper;

        private IQueryable<PredictionSchedule> PredictionScheduleQuery =>
            _dbContext.Query<PredictionSchedule>()
                .Include(o => o.Ratings);

        public RatingsScheduleRepository(
            ISqlServerDbContext dbContext,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper
            )
        {
            _dbContext = dbContext;
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
            _mapper = mapper;
        }

        public int CountAll => _dbContext.Query<PredictionSchedule>().Count();

        public RatingsPredictionSchedule GetSchedule(DateTime fromDateTime, string salesarea) =>
            _mapper.Map<RatingsPredictionSchedule>(
                PredictionScheduleQuery
                .FirstOrDefault(x => x.SalesArea.Name == salesarea && x.ScheduleDay == fromDateTime),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public List<RatingsPredictionSchedule> GetSchedules(DateTime fromDateTime, DateTime toDateTime, string salesarea) =>
            _mapper.Map<List<RatingsPredictionSchedule>>(
                PredictionScheduleQuery
                .Where(x => x.ScheduleDay <= toDateTime.Date && x.ScheduleDay >= fromDateTime.Date && x.SalesArea.Name == salesarea).AsNoTracking(),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public IEnumerable<RatingsPredictionSchedule> GetAll() =>
            _mapper.Map<List<RatingsPredictionSchedule>>(
                PredictionScheduleQuery.AsNoTracking(),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

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
                _ = _dbContext.Add(
                    _mapper.Map<PredictionSchedule>(item, opts => opts.UseEntityCache(_salesAreaByNameCache)),
                    post => post.MapTo(item, mappingOptions => mappingOptions.UseEntityCache(_salesAreaByIdCache)), _mapper);
            }
            else
            {
                _ = _mapper.Map(item, entity, opts => opts.UseEntityCache(_salesAreaByNameCache));
                _ = _dbContext.Update(entity, post => post.MapTo(item, mappingOptions => mappingOptions.UseEntityCache(_salesAreaByIdCache)), _mapper);
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

            var entities = _mapper.Map<List<PredictionSchedule>>(items, opts => opts.UseEntityCache(_salesAreaByNameCache));
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
                actionBuilder.TryToUpdate(items, mappingOptions => mappingOptions.UseEntityCache(_salesAreaByIdCache));
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
                    salesAreaNames.Contains(r.PredictionSchedule.SalesArea.Name) &&
                    demographics.Contains(r.Demographic))
                .GroupBy(r => new { r.PredictionSchedule.SalesArea.Name, r.PredictionSchedule.ScheduleDay, r.Demographic })
                .Select(g => new
                {
                    SalesAreaName = g.Key.Name,
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
                messages.Add(new RatingsPredictionValidationMessage
                {
                    Message = $"Count: {item.Count} for Demographic: {item.Demographic}, incorrect in Sales Area: " +
                    $"{item.SalesAreaName}, in Schedule Day: {item.ScheduleDay.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}"
                });
            }

            // check for missing whole documents by date and sales area from the above in memory collection
            var missingRatingPredictionDays = new List<string>();
            foreach (string salesAreaName in salesAreaNames)
            {
                for (int i = 0; i < noOfDaysInRun; i++)
                {
                    var ratingsScheduleForThisDay = allRatingsScheduleForDemographicsEnabled.Where(r =>
                        r.SalesAreaName == salesAreaName
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
