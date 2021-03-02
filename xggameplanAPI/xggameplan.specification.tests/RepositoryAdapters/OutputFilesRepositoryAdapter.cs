using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.OutputFiles;
using ImagineCommunications.GamePlan.Domain.OutputFiles.Objects;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class OutputFilesRepositoryAdapter : RepositoryTestAdapter<OutputFile, IOutputFileRepository, string>
    {
        public OutputFilesRepositoryAdapter(IScenarioDbContext dbContext, IOutputFileRepository repository)
            : base(dbContext, repository)
        {
        }

        protected override OutputFile Add(OutputFile model)
        {
            Repository.Insert(model);
            return model;
        }

        protected override IEnumerable<OutputFile> GetAll() =>
            Repository.GetAll();

        protected override OutputFile GetById(string id) =>
            Repository.Find(id);

        protected override IEnumerable<OutputFile> AddRange(params OutputFile[] models) =>
            throw new NotImplementedException();

        protected override int Count() =>
            throw new NotImplementedException();

        protected override void Delete(string id) =>
            throw new NotImplementedException();

        protected override void Truncate() =>
            throw new NotImplementedException();

        protected override OutputFile Update(OutputFile model) =>
            throw new NotImplementedException();
    }
}
