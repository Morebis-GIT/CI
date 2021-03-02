using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.BRS;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class BRSConfigurationTemplateRepository: IBRSConfigurationTemplateRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public BRSConfigurationTemplateRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void Add(BRSConfigurationTemplate item) => _dbContext.Add(_mapper.Map<Entities.Tenant.BRS.BRSConfigurationTemplate>(item), post => post.MapTo(item), _mapper);

        public void Update(BRSConfigurationTemplate item)
        {
            var entity = _dbContext.Find<Entities.Tenant.BRS.BRSConfigurationTemplate>(new object[] { item.Id }, find => find.IncludeCollection(x => x.KPIConfigurations));
            if (entity is null)
            {
                return;
            }

            _mapper.Map(item, entity);
            _dbContext.Update(entity, post => post.MapTo(item), _mapper);
        }

        public void Delete(int id)
        {
            var entity = _dbContext.Find<Entities.Tenant.BRS.BRSConfigurationTemplate>(id);
            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public BRSConfigurationTemplate Get(int id)
        {
            var item = _dbContext.Find<Entities.Tenant.BRS.BRSConfigurationTemplate>(new object[] { id }, find => find.IncludeCollection(x => x.KPIConfigurations));
            return _mapper.Map<BRSConfigurationTemplate>(item);
        }

        public BRSConfigurationTemplate GetByName(string name)
        {
            return _dbContext
                .Query<Entities.Tenant.BRS.BRSConfigurationTemplate>()
                .ProjectTo<BRSConfigurationTemplate>(_mapper.ConfigurationProvider)
                .FirstOrDefault(x => x.Name == name);
        }

        public IEnumerable<BRSConfigurationTemplate> GetAll()
        {
            return _dbContext
                .Query<Entities.Tenant.BRS.BRSConfigurationTemplate>()
                .ProjectTo<BRSConfigurationTemplate>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public BRSConfigurationTemplate GetDefault()
        {
            var item = _dbContext
                .Query<Entities.Tenant.BRS.BRSConfigurationTemplate>()
                .Include(x => x.KPIConfigurations)
                .FirstOrDefault(x => x.IsDefault);
            return _mapper.Map<BRSConfigurationTemplate>(item);
        }

        public bool ChangeDefaultConfiguration(int id)
        {
            var newDefault = _dbContext.Find<Entities.Tenant.BRS.BRSConfigurationTemplate>(id);
            if (newDefault == null)
            {
                return false;
            }

            var oldDefault = _dbContext
                .Query<Entities.Tenant.BRS.BRSConfigurationTemplate>()
                .First(x => x.IsDefault);

            oldDefault.IsDefault = false;
            newDefault.IsDefault = true;
            return true;
        }

        public int Count() => _dbContext.Query<Entities.Tenant.BRS.BRSConfigurationTemplate>().Count();

        public bool Exists(int id) =>
            _dbContext.Query<Entities.Tenant.BRS.BRSConfigurationTemplate>().ProjectTo<BRSConfigurationTemplate>(_mapper.ConfigurationProvider).Any(t => t.Id == id);

        public void SaveChanges() => _dbContext.SaveChanges();
    }
}
