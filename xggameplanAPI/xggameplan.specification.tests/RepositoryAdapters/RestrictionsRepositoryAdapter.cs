using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Queries;
using xggameplan.specification.tests.Infrastructure.RepositoryMethod;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class RestrictionsRepositoryAdapter : RepositoryTestAdapter<Restriction, IRestrictionRepository, Guid>
    {
        public RestrictionsRepositoryAdapter(IScenarioDbContext dbContext, IRestrictionRepository repository) : base(dbContext, repository)
        {
        }

        protected override Restriction Add(Restriction model)
        {
            Repository.Add(model);

            return model;
        }

        protected override IEnumerable<Restriction> AddRange(params Restriction[] models)
        {
            Repository.Add(models);
            return models;
        }

        protected override int Count()
        {
            throw new NotImplementedException();
        }

        protected override void Delete(Guid id)
        {
            Repository.Delete(id);
        }

        protected override IEnumerable<Restriction> GetAll() =>
            Repository.GetAll();

        protected override Restriction GetById(Guid id)
        {
            return Repository.Get(id);
        }

        protected override void Truncate()
        {
            Repository.Truncate();
        }

        protected override Restriction Update(Restriction model)
        {
            throw new NotImplementedException();
        }

        [RepositoryMethod]
        protected CallMethodResult GetDesc(Guid id)
        {
            var result = Repository.GetDesc(id);

            TestContext.LastSingleResult = result?.Item2;
            TestContext.LastOperationCount = result != null ? 1 : 0;
            TestContext.LastCollectionResult = null;

            return CallMethodResult.CreateHandled();
        }

        [RepositoryMethod]
        protected CallMethodResult SearchRestrictions(
            List<string> salesAreaNames,
            DateTime? startDate,
            DateTime? endDate,
            RestrictionType? restrictionType,
            bool matchAllSpecifiedSalesAreas)
        {
            var queryModel = new RestrictionSearchQueryModel
            {
                SalesAreaNames = salesAreaNames,
                DateRangeStart = startDate,
                DateRangeEnd = endDate,
                RestrictionType = restrictionType,
                MatchAllSpecifiedSalesAreas = matchAllSpecifiedSalesAreas
            };
            var res = Repository.Get(queryModel);

            TestContext.LastOperationCount = res?.Items?.Count ?? 0;
            TestContext.LastCollectionResult = res?.Items;
            TestContext.LastSingleResult = null;

            return CallMethodResult.CreateHandled();
        }

        [RepositoryMethod]
        protected CallMethodResult DeleteByCriteria(
            List<string> salesAreaNames,
            DateTime? startDate,
            DateTime? endDate,
            RestrictionType? restrictionType,
            bool matchAllSpecifiedSalesAreas)
        {
            DbContext.WaitForIndexesAfterSaveChanges();
            Repository.Delete(salesAreaNames, matchAllSpecifiedSalesAreas, startDate, endDate, restrictionType);
            DbContext.SaveChanges();
            return CallMethodResult.CreateHandled();
        }

        [RepositoryMethod]
        protected CallMethodResult UpdateRange(Guid uid, DateTime startDate, DateTime? endDate,
            string restrictionDays, TimeSpan? startTime, TimeSpan? endTime,
            int timeToleranceMinsAfter, int timeToleranceMinsBefore,
            IncludeOrExcludeOrEither schoolHolidayIndicator, IncludeOrExcludeOrEither publicHolidayIndicator)
        {
            DbContext.WaitForIndexesAfterSaveChanges();

            var restriction = GetById(uid);
            restriction.StartDate = startDate;
            restriction.EndDate = endDate;
            restriction.RestrictionDays = restrictionDays;
            restriction.StartTime = startTime;
            restriction.EndTime = endTime;
            restriction.TimeToleranceMinsAfter = timeToleranceMinsAfter;
            restriction.TimeToleranceMinsBefore = timeToleranceMinsBefore;
            restriction.SchoolHolidayIndicator = schoolHolidayIndicator;
            restriction.PublicHolidayIndicator = publicHolidayIndicator;

            Repository.UpdateRange(new[] { restriction });
            DbContext.SaveChanges();
            return CallMethodResult.CreateHandled();
        }
    }
}
