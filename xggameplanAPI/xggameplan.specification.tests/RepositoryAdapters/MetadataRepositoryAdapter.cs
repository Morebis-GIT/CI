using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;
using xggameplan.specification.tests.Infrastructure.RepositoryMethod;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class MetadataRepositoryAdapter : RepositoryTestAdapter<MetadataModel, IMetadataRepository, int>
    {
        public MetadataRepositoryAdapter(IScenarioDbContext dbContext, IMetadataRepository repository) : base(dbContext, repository)
        {
        }

        protected override MetadataModel Add(MetadataModel model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<MetadataModel> GetAll()
        {
            var res = Repository.GetAll();
            return res != null ? new List<MetadataModel> { res } : Enumerable.Empty<MetadataModel>();
        }

        [RepositoryMethod]
        protected CallMethodResult GetByKeys(List<MetaDataKeys> keys)
        {
            var res = Repository.GetByKeys(keys);

            TestContext.LastOperationCount = res?.SelectMany(x => x.Value).Count() ?? 0;
            TestContext.LastCollectionResult = null;
            TestContext.LastSingleResult = null;

            return CallMethodResult.CreateHandled();
        }

        [RepositoryMethod]
        protected CallMethodResult DeleteByKey(MetaDataKeys key)
        {
            DbContext.WaitForIndexesAfterSaveChanges();
            Repository.DeleteByKey(key);
            DbContext.SaveChanges();
            return CallMethodResult.CreateHandled();
        }

        protected override void Delete(int id) => throw new NotImplementedException();

        protected override void Truncate() => throw new NotImplementedException();

        protected override int Count() => throw new NotImplementedException();

        protected override IEnumerable<MetadataModel> AddRange(params MetadataModel[] models) =>
            throw new NotImplementedException();

        protected override MetadataModel Update(MetadataModel model) => throw new NotImplementedException();

        protected override MetadataModel GetById(int id) => throw new NotImplementedException();
    }
}
