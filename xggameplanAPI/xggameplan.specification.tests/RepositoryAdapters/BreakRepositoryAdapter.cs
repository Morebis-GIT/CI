using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using xggameplan.specification.tests.Extensions;
using xggameplan.specification.tests.Infrastructure.RepositoryMethod;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class BreakRepositoryAdapter : RepositoryTestAdapter<Break, IBreakRepository, Guid>
    {
        public BreakRepositoryAdapter(IScenarioDbContext dbContext, IBreakRepository repository) : base(dbContext, repository)
        {
        }

        protected override Break Add(Break model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<Break> AddRange(params Break[] models)
        {
            Repository.Add(models);
            return models;
        }

        protected override Break Update(Break model)
        {
            throw new NotImplementedException();
        }

        protected override Break GetById(Guid id)
        {
            return Repository.Get(id);
        }

        protected override IEnumerable<Break> GetAll()
        {
            return Repository.GetAll();
        }

        protected override void Delete(Guid id)
        {
            Repository.Delete(id);
        }

        protected override void Truncate()
        {
            Repository.TruncateAsync().Wait();
        }

        protected override int Count()
        {
            return Repository.Count();
        }

        [RepositoryMethod]
        protected CallMethodResult SearchBySalesAreas(IDictionary<string, string> parameters)
        {
            return CallMethodResult.Create(
                Repository.Search(
                    parameters.GetBySpecflowService<DateTimeRange>("scheduledDatesRange"),
                    parameters.GetBySpecflowService<List<string>>("salesAreaNames")));
        }

        [RepositoryMethod]
        protected CallMethodResult Delete(IDictionary<string, string> parameters)
        {
            DbContext.WaitForIndexesAfterSaveChanges();
            Repository.Delete(parameters.GetBySpecflowService<List<string>>("ids")
                .Select(x => x.Trim().SpecflowConvert<Guid>())
                .ToList());
            DbContext.SaveChanges();

            return CallMethodResult.CreateHandled();
        }
    }
}
