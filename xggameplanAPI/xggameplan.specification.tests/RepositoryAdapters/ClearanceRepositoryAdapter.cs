using System;
using System.Collections.Generic;
using AutoFixture.Dsl;
using ImagineCommunications.GamePlan.Domain.Shared.ClearanceCodes;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class ClearanceRepositoryAdapter : RepositoryTestAdapter<ClearanceCode, IClearanceRepository, int>
    {
        private Random _idRandomizer = new Random();

        public ClearanceRepositoryAdapter(IScenarioDbContext dbContext, IClearanceRepository repository) : base(dbContext, repository)
        {
        }

        protected override ClearanceCode Add(ClearanceCode model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<ClearanceCode> AddRange(params ClearanceCode[] models)
        {
            Repository.Add(models);
            return models;
        }

        protected override ClearanceCode Update(ClearanceCode model)
        {
            throw new NotImplementedException();
        }

        protected override ClearanceCode GetById(int id)
        {
            return Repository.Find(id);
        }

        protected override IEnumerable<ClearanceCode> GetAll()
        {
            return Repository.GetAll();
        }

        protected override void Delete(int id)
        {
            Repository.Remove(id, out var isDeleted);
        }

        protected override void Truncate()
        {
            Repository.Truncate();
        }

        protected override int Count()
        {
            return Repository.Count(cc => cc.Id != 0);
        }

        protected override IPostprocessComposer<ClearanceCode> GetAutoModelComposer()
        {
            return base.GetAutoModelComposer()
                .With(x => x.Id, () => _idRandomizer.Next(1, int.MaxValue));
        }
    }
}
