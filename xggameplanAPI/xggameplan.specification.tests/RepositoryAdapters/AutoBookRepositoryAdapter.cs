using System;
using System.Collections.Generic;
using AutoFixture.Dsl;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage.Objects;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class AutoBookRepositoryAdapter : RepositoryTestAdapter<AutoBook, IAutoBookRepository, string>
    {
        public AutoBookRepositoryAdapter(IScenarioDbContext dbContext, IAutoBookRepository repository)
            : base(dbContext, repository)
        {
        }

        protected override IPostprocessComposer<AutoBook> GetAutoModelComposer()
        {
            return base.GetAutoModelComposer().With(x => x.Id, () => Guid.NewGuid().ToString());
        }

        protected override AutoBook Add(AutoBook model)
        {
            Repository.Add(model);
            return model;
        }

        protected override AutoBook Update(AutoBook model)
        {
            Repository.Update(model);
            return model;
        }

        protected override void Delete(string id) => Repository.Delete(id);

        protected override IEnumerable<AutoBook> GetAll() => Repository.GetAll();

        protected override AutoBook GetById(string id) => Repository.Get(id);

        protected override IEnumerable<AutoBook> AddRange(params AutoBook[] models) => throw new NotImplementedException();

        protected override int Count() => Repository.CountAll;

        protected override void Truncate() => throw new NotImplementedException();
    }
}
