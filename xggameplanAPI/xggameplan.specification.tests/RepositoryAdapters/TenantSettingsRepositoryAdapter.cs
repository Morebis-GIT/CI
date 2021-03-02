using System;
using System.Collections.Generic;
using AutoFixture.Dsl;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using xggameplan.specification.tests.Infrastructure.RepositoryMethod;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class TenantSettingsRepositoryAdapter : RepositoryTestAdapter<TenantSettings, ITenantSettingsRepository, Guid>
    {
        protected class GuidResult
        {
            public Guid Id { get; set; }
        }

        protected override IPostprocessComposer<TenantSettings> GetAutoModelComposer()
        {
            return base.GetAutoModelComposer()
                .With(x => x.PeakStartTime, "180000")
                .With(x => x.PeakEndTime, "220000")
                .With(x => x.MidnightStartTime, "240000")
                .With(x => x.MidnightEndTime, "995959");
        }

        public TenantSettingsRepositoryAdapter(IScenarioDbContext dbContext, ITenantSettingsRepository repository) : base(
            dbContext, repository)
        {
        }

        protected override TenantSettings Add(TenantSettings model)
        {
            Repository.AddOrUpdate(model);
            return model;
        }

        protected override IEnumerable<TenantSettings> AddRange(params TenantSettings[] models)
        {
            throw new NotImplementedException();
        }

        protected override TenantSettings Update(TenantSettings model)
        {
            Repository.AddOrUpdate(model);
            return model;
        }

        protected override TenantSettings GetById(Guid id)
        {
            return Repository.Get();
        }

        protected override IEnumerable<TenantSettings> GetAll()
        {
            throw new NotImplementedException();
        }

        protected override void Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        protected override void Truncate()
        {
            throw new NotImplementedException();
        }

        protected override int Count()
        {
            throw new NotImplementedException();
        }

        [RepositoryMethod]
        protected CallMethodResult GetDefaultSalesAreaPassPriorityId(Guid id)
        {
            return CallMethodResult.Create(new GuidResult
            {
                Id = Repository.GetDefaultSalesAreaPassPriorityId()
            });
        }

        [RepositoryMethod]
        protected CallMethodResult GetDefaultScenarioId(Guid id)
        {
            return CallMethodResult.Create(new GuidResult
            {
                Id = Repository.GetDefaultScenarioId()
            });
        }
    }
}
