using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.OutputFiles;
using ImagineCommunications.GamePlan.Domain.OutputFiles.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class OutputFileRepository : IOutputFileRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public OutputFileRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public OutputFile Find(string id)
        {
            return _dbContext.Query<Entities.Tenant.OutputFiles.OutputFile>().ProjectTo<OutputFile>(_mapper.ConfigurationProvider)
                .FirstOrDefault(x => x.FileId == id);
        }

        public List<OutputFile> GetAll()
        {
            return _dbContext.Query<Entities.Tenant.OutputFiles.OutputFile>().ProjectTo<OutputFile>(_mapper.ConfigurationProvider).ToList();
        }

        public void Insert(OutputFile outputFile)
        {
            if (outputFile == null)
            {
                throw new ArgumentNullException(nameof(outputFile));
            }

            var entity = _dbContext.Query<Entities.Tenant.OutputFiles.OutputFile>().Include(x => x.Columns)
                .FirstOrDefault(x => x.FileId == outputFile.FileId);
            if (entity == null)
            {
                _dbContext.Add(_mapper.Map<Entities.Tenant.OutputFiles.OutputFile>(outputFile),
                    post => post.MapTo(outputFile), _mapper);
            }
            else
            {
                _mapper.Map(outputFile, entity);
                _dbContext.Update(entity, post => post.MapTo(outputFile), _mapper);
            }
        }
    }
}
