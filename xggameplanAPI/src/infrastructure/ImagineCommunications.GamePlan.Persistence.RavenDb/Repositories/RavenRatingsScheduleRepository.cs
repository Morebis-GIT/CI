using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Transformers;
using Raven.Client;
using Raven.Client.Linq;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenRatingsScheduleRepository : IRatingsScheduleRepository
    {
        private readonly IDocumentSession _session;
        private readonly IAsyncDocumentSession _sessionAsync;

        public RavenRatingsScheduleRepository(IDocumentSession session, IAsyncDocumentSession sessionAsync)
        {
            _session = session;
            _sessionAsync = sessionAsync;
        }

        public IEnumerable<RatingsPredictionSchedule> GetAll()
        {
            return _session.Query<RatingsPredictionSchedule>().Take(int.MaxValue).ToList();
        }

        public int CountAll
        {
            get
            {
                lock (_session)
                {
                    return _session.CountAll<RatingsPredictionSchedule>();
                }
            }
        }

        /// <summary>
        /// Add new entry in schedule
        /// </summary>
        /// <param name="item"></param>
        public void Add(RatingsPredictionSchedule item)
        {
            _session.Store(item);
        }

        public void Insert(List<RatingsPredictionSchedule> items, bool setIdentity = true)
        {
            using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert())
            {
                items.ToList().ForEach(item => _session.Store(item));
            }
        }

        public void Remove(RatingsPredictionSchedule item)
        {
            _session.Delete(item);
        }

        public void DeleteRange(IEnumerable<int> ids)
        {
            lock (_session)
            {
                var rules = _session.GetAll<RatingsPredictionSchedule>(s => s.Id.In(ids.ToList()));
                foreach (var rule in rules)
                {
                    _session.Delete(rule);
                }
            }
        }

        /// <summary>
        /// GetSchedule by salesarea and scheduledate
        /// </summary>
        /// <param name="salesarea"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public RatingsPredictionSchedule GetSchedule(DateTime date, string salesarea)
        {
            return _session.Query<RatingsPredictionSchedule>(
                indexName: RatingsPredictionsSchedules_BySalesAreaScheduleDay.DefaultIndexName,
                isMapReduce: false
                ).FirstOrDefault(s => s.SalesArea == salesarea && s.ScheduleDay == date);
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
            var items = _session.GetAll<RatingsPredictionSchedule>(p => p.ScheduleDay <= toDateTime.Date && p.ScheduleDay >= fromDateTime.Date && p.SalesArea == salesarea,
                                  indexName: RatingsPredictionsSchedules_BySalesAreaScheduleDay.DefaultIndexName,
                                  isMapReduce: false);
            return items;
        }

        /// <summary>
        /// Checks the Demographic/SalesArea/ScheduleDay where there are not ratings prediction for all 96 slots in a day (4 x 24).
        /// Checks that there are the expected number of documents for each date and sales area in the run.
        /// </summary>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="salesAreaNames"></param>
        /// <param name="demographicsNames"></param>
        /// <param name="noOfRatingPredictionsPerScheduleDayAreaDemo"></param>
        /// <returns>List of messages for any errors in the document collection </returns>
        public List<RatingsPredictionValidationMessage> Validate_RatingsPredictionSchedules(DateTime fromDateTime, DateTime toDateTime,
            IEnumerable<string> salesAreaNames, IEnumerable<string> demographicsExtRefs, int noOfRatingPredictionsPerScheduleDayAreaDemo)
        {
            TimeSpan spanOfDays = toDateTime.Date - fromDateTime.Date;
            int noOfDaysInRun = spanOfDays.Days + 1; //inclusive of fromDate

            var messages = new List<RatingsPredictionValidationMessage>();

            lock (_session)
            {
                //get all the ratingspredictionschedules documents (transformed group of salesArea,scheduledDay,demographic - count) by date range of run and all the sales areas passed in
                //added 1 day to toDateTime in order to overcome the wierd comparison result of [schedule.ScheduleDay <= toDateTime.Date] where the last day of data was missing
                var allRatingsScheduleInDateRange = _session.GetAllWithTransform<RatingsPredictionSchedule, RatingsPredictionSchedule_TransformerRating,
                    RatingsPredictionSchedule_TransformerRating.Result>
                    (schedule => schedule.ScheduleDay >= fromDateTime.Date && schedule.ScheduleDay <= toDateTime.Date.AddDays(1) && schedule.SalesArea.In(salesAreaNames),
                    indexName: RatingsPredictionsSchedules_BySalesAreaScheduleDay.DefaultIndexName, isMapReduce: false);

                //then filter by the demographics enabled for gameplan
                var allRatingsScheduleForDemographicsEnabled = allRatingsScheduleInDateRange.Where(r => r.Demographic.In(demographicsExtRefs)).ToList();

                //check that we have 96 (4 x 24) ratings for each day for each sales area, report where we dont
                //(this could be less than 96 where individual ratings are missing from a document or more than 96 say 192 for duplicate documents)
                var _incorrectRatings = allRatingsScheduleForDemographicsEnabled.Where(r => r.Count != noOfRatingPredictionsPerScheduleDayAreaDemo).ToList();
                foreach (var item in _incorrectRatings)
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
        }

        [Obsolete("Try to use TruncateAsync() as it is more reliable.")]
        public void Truncate()
        {
            _session.TryDelete("Raven/DocumentsByEntityName", "RatingsPredictionSchedules");
        }

        public async Task TruncateAsync()
        {
            const int maximumTimeoutSeconds = 180;
            const int retryMillisecondDelay = 100;
            const int maximumNumberOfRetries = 100;

            var maximumSecondsWaitingForNonStaleIndexes = TimeSpan.FromSeconds(maximumTimeoutSeconds);
            int remainingRetries = maximumNumberOfRetries;
            bool retry = false;
            var startTime = DateTime.UtcNow;

            do
            {
                retry = false;

                try
                {
                    var operation = await _sessionAsync.Advanced
                        .DeleteByIndexAsync<RatingsPredictionSchedule, RatingsPredictionsSchedules_BySalesAreaScheduleDay>(ForceDelete())
                        .ConfigureAwait(false);

                    await operation
                        .WaitForCompletionAsync()
                        .ConfigureAwait(false);
                }
                catch (Exception ex) when (CanRetry(ex))
                {
                    remainingRetries--;
                    retry = true;

                    await Task.Delay(retryMillisecondDelay)
                        .ConfigureAwait(false);

                    continue;
                }
                catch (Exception ex) when (remainingRetries == 0)
                {
                    throw new RepositoryException(
                        $"Deleting all RatingsPredictionSchedule documents stopped after {maximumNumberOfRetries} attempts. Wait for a few minutes and try again.",
                        ex
                    );
                }
                catch (Exception ex) when (IndexIsStale(ex))
                {
                    throw new RepositoryException(
                        $"Deleting all RatingsPredictionSchedule documents timed out after {maximumTimeoutSeconds} seconds as the index is stale. Wait for a few minutes and try again.",
                        ex
                    );
                }
            } while (retry);

            bool IndexIsStale(Exception ex) => ex.Message.Contains("index is stale");

            bool MaximumTimeToWaitForIndexesExceeded() =>
                DateTime.UtcNow - startTime > maximumSecondsWaitingForNonStaleIndexes;

            bool CanRetry(Exception ex) =>
                IndexIsStale(ex) &&
                remainingRetries > 0 &&
                !MaximumTimeToWaitForIndexesExceeded();

            Expression<Func<RatingsPredictionSchedule, bool>> ForceDelete() =>
                rps => rps.SalesArea != string.Empty;
        }

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }
    }
}
