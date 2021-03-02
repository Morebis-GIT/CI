using System;
using System.Collections.Generic;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class AutoBookDefaultParametersRepositoryAdapter : RepositoryTestAdapter<IAutoBookDefaultParameters, IAutoBookDefaultParametersRepository, Guid>
    {
        public AutoBookDefaultParametersRepositoryAdapter(
            IScenarioDbContext dbContext,
            IAutoBookDefaultParametersRepository repository
        ) : base(dbContext, repository) 
        {
            Fixture.Customizations.Add(new TypeRelay(
                typeof(IAutoBookDefaultParameters),
                typeof(AutoBookDefaultParameters)));
        }

        protected override IPostprocessComposer<IAutoBookDefaultParameters> GetAutoModelComposer()
        {
            return new NodeComposer<IAutoBookDefaultParameters>(Fixture.Build<AutoBookDefaultParameters>()
                .WithAutoProperties());
        }

        protected override IAutoBookDefaultParameters Add(IAutoBookDefaultParameters model)
        {
            Repository.AddOrUpdate(model);
            return model;
        }

        protected override IEnumerable<IAutoBookDefaultParameters> AddRange(params IAutoBookDefaultParameters[] models) =>
            throw new NotImplementedException();

        protected override int Count() =>
            throw new NotImplementedException();

        protected override void Delete(Guid id) =>
            throw new NotImplementedException();

        protected override IEnumerable<IAutoBookDefaultParameters> GetAll() =>
            DbContext.GetAll<AutoBookDefaultParameters>();

        protected override IAutoBookDefaultParameters GetById(Guid id) =>
            Repository.Get();

        protected override void Truncate() =>
            throw new NotImplementedException();

        protected override IAutoBookDefaultParameters Update(IAutoBookDefaultParameters model)
        {
            Repository.AddOrUpdate(model);
            return model;
        }
    }
}
