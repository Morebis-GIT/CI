using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.OutputFiles;
using ImagineCommunications.GamePlan.Domain.OutputFiles.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using OutputFileEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.OutputFiles.OutputFile;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class OutputFileDomainModelHandler : IDomainModelHandler<OutputFile>
    {
        private readonly IOutputFileRepository _outputFileRepository;
        private readonly ISqlServerDbContext _dbContext;

        public OutputFileDomainModelHandler(IOutputFileRepository outputFileRepository, ISqlServerDbContext dbContext)
        {
            _outputFileRepository = outputFileRepository ??
                throw new ArgumentNullException(nameof(outputFileRepository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public OutputFile Add(OutputFile model)
        {
            _outputFileRepository.Insert(model);
            return model;
        }

        public void AddRange(params OutputFile[] models)
        {
            foreach (var model in models)
            {
                _outputFileRepository.Insert(model);
            }
        }

        public int Count() => _dbContext.Query<OutputFileEntity>().Count();

        public void DeleteAll() => _dbContext.Truncate<OutputFileEntity>();

        public IEnumerable<OutputFile> GetAll() => _outputFileRepository.GetAll();
    }
}
