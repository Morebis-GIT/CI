using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Shared.System.Preview;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Services;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class TenantsRepository : ITenantsRepository
    {
        private readonly ISqlServerMasterDbContext _dbContext;
        private readonly SqlServerPreviewFileStorage<Entities.Master.Tenant> _previewFileStorage;
        private readonly IMapper _mapper;

        public TenantsRepository(ISqlServerMasterDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _previewFileStorage = new SqlServerPreviewFileStorage<Entities.Master.Tenant>(dbContext, mapper);
        }

        public void SaveChanges() =>
            _dbContext.SaveChanges();

        public Tenant GetById(int id) =>
            _mapper.Map<Tenant>(_dbContext.Find<Entities.Master.Tenant>(id));

        public List<Tenant> GetAll() =>
            _dbContext.Query<Entities.Master.Tenant>()
            .ProjectTo<Tenant>(_mapper.ConfigurationProvider)
            .ToList();

        public void Add(Tenant tenant) =>
            _dbContext.Add(_mapper.Map<Entities.Master.Tenant>(tenant),
                post => post.MapTo(tenant), _mapper);

        public void Update(Tenant tenant)
        {

            var entity = _dbContext.Find<Entities.Master.Tenant>(
                   (tenant ?? throw new ArgumentNullException(nameof(tenant))).Id);
            if (entity != null)
            {
                _mapper.Map(tenant, entity);
                _dbContext.Update(_mapper.Map<Entities.Master.Tenant>(entity),
                    post => post.MapTo(tenant), _mapper);
            }
        }

        public PreviewFile GetPreviewFile(int entityId, out Stream previewFileStream) =>
              _previewFileStorage.GetPreviewFile(entityId, out previewFileStream);

        public void SetPreviewFile(int entityId, PreviewFile previewFile, Stream previewFileStream) =>
            _previewFileStorage.SetPreviewFile(entityId, previewFile, previewFileStream);

        public void DeletePreviewFile(int entityId) => _previewFileStorage.DeletePreviewFile(entityId);
        public void Flush() => _previewFileStorage.Flush();
    }
}
