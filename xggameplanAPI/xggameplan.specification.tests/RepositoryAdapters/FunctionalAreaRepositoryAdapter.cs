using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas.Objects;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class FunctionalAreaRepositoryAdapter
        : RepositoryTestAdapter<FunctionalArea, IFunctionalAreaRepository, Guid>
    {
        private int _faultTypeIdValue;
        protected int FaultTypeIdGenerator => ++_faultTypeIdValue;

        public FunctionalAreaRepositoryAdapter(IScenarioDbContext dbContext, IFunctionalAreaRepository repository)
            : base(dbContext, repository)
        {
            Fixture.Customize<FaultType>(composer => composer
                .With(p => p.Id, () => FaultTypeIdGenerator));
        }

        protected override FunctionalArea Add(FunctionalArea model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<FunctionalArea> AddRange(params FunctionalArea[] models) =>
            throw new NotImplementedException();

        protected override int Count() =>
            throw new NotImplementedException();

        protected override void Delete(Guid id) =>
            throw new NotImplementedException();

        protected override IEnumerable<FunctionalArea> GetAll() =>
            Repository.GetAll();

        protected override FunctionalArea GetById(Guid id) =>
            Repository.Find(id);

        protected override void Truncate() =>
            throw new NotImplementedException();

        protected override FunctionalArea Update(FunctionalArea model)
        {
            Repository.UpdateFaultTypesSelections(model);
            return model;
        }
    }
}
