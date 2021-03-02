using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.DbSequence;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Helpers;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class SalesAreaRepository : ISalesAreaRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IIdentityGenerator _identityGenerator;
        private readonly SequenceRebuilder<Entities.Tenant.SalesAreas.SalesArea, SalesAreaNoIdentity> _sequenceRebuilder;
        private readonly IMapper _mapper;

        public SalesAreaRepository(
            ISqlServerTenantDbContext dbContext,
            IIdentityGenerator identityGenerator,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _identityGenerator = identityGenerator;
            _sequenceRebuilder = new SequenceRebuilder<Entities.Tenant.SalesAreas.SalesArea, SalesAreaNoIdentity>();
            _mapper = mapper;
        }

        public int CountAll => _dbContext.Query<Entities.Tenant.SalesAreas.SalesArea>().Count();

        public void Add(Domain.Shared.SalesAreas.SalesArea salesArea)
        {
            salesArea.CustomId = _identityGenerator.GetIdentities<SalesAreaNoIdentity>(1).First().Id;
            _dbContext.Add(_mapper.Map<Entities.Tenant.SalesAreas.SalesArea>(salesArea),
                post => post.MapTo(salesArea), _mapper);
        }

        public void DeleteByShortName(string shortName)
        {
            var existingSalesAreas = _dbContext.Query<Entities.Tenant.SalesAreas.SalesArea>()
                .Where(x => x.ShortName == shortName)
                .ToArray();

            if (existingSalesAreas.Any())
            {
                _dbContext.RemoveRange(existingSalesAreas);
            }
        }

        public void DeleteRange(IEnumerable<Guid> ids)
        {
            var existingSalesAreas = _dbContext.Query<Entities.Tenant.SalesAreas.SalesArea>()
                .Where(x => ids.Contains(x.Id))
                .ToArray();

            if (existingSalesAreas.Any())
            {
                _dbContext.RemoveRange(existingSalesAreas);
            }
        }

        public Domain.Shared.SalesAreas.SalesArea Find(Guid id) =>
            _mapper.Map<Domain.Shared.SalesAreas.SalesArea>(_dbContext.Find<Entities.Tenant.SalesAreas.SalesArea>(new object[] { id },
                find => find.IncludeCollection(sa => sa.ChannelGroups).IncludeCollection(sa => sa.Holidays)));

        public List<Domain.Shared.SalesAreas.SalesArea> FindByIds(List<int> Ids) =>
            _dbContext.Query<Entities.Tenant.SalesAreas.SalesArea>()
                .Where(x => Ids.Contains(x.CustomId))
                .ProjectTo<Domain.Shared.SalesAreas.SalesArea>(_mapper.ConfigurationProvider).ToList();

        public Domain.Shared.SalesAreas.SalesArea FindByName(string name) =>
            _mapper.Map<Domain.Shared.SalesAreas.SalesArea>(_dbContext.Query<Entities.Tenant.SalesAreas.SalesArea>()
                .ProjectTo<Domain.Shared.SalesAreas.SalesArea>(_mapper.ConfigurationProvider)
                .FirstOrDefault(x => x.Name == name));

        public List<Domain.Shared.SalesAreas.SalesArea> FindByNames(List<string> names) =>
            _dbContext.Query<Entities.Tenant.SalesAreas.SalesArea>()
                .Where(x => names.Contains(x.Name)).ProjectTo<Domain.Shared.SalesAreas.SalesArea>(_mapper.ConfigurationProvider).ToList();

        public Domain.Shared.SalesAreas.SalesArea FindByShortName(string shortName) =>
            _dbContext.Query<Entities.Tenant.SalesAreas.SalesArea>()
                .ProjectTo<Domain.Shared.SalesAreas.SalesArea>(_mapper.ConfigurationProvider)
                .FirstOrDefault(x => x.ShortName == shortName);

        public IEnumerable<Domain.Shared.SalesAreas.SalesArea> FindByShortNames(IEnumerable<string> shortNames) =>
            _dbContext.Query<Entities.Tenant.SalesAreas.SalesArea>()
                .Where(x => shortNames.Contains(x.ShortName))
                .ProjectTo<Domain.Shared.SalesAreas.SalesArea>(_mapper.ConfigurationProvider)
                .ToList();

        public IEnumerable<Domain.Shared.SalesAreas.SalesArea> GetAll() =>
            _dbContext.Query<Entities.Tenant.SalesAreas.SalesArea>()
                .ProjectTo<Domain.Shared.SalesAreas.SalesArea>(_mapper.ConfigurationProvider).ToList();

        public List<string> GetListOfNames(List<Domain.Shared.SalesAreas.SalesArea> salesAreas) =>
            salesAreas.Select(item => item.Name).ToList();

        public List<string> GetListOfNames() =>
            _dbContext.Query<Entities.Tenant.SalesAreas.SalesArea>().Select(x => x.Name).ToList();

        public void Remove(Guid id)
        {
            var entity = _dbContext.Find<Entities.Tenant.SalesAreas.SalesArea>(id);
            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
            _sequenceRebuilder.Execute(_dbContext, e => e.CustomId);
        }

        public void Update(Domain.Shared.SalesAreas.SalesArea salesArea)
        {
            var entity = _dbContext.Find<Entities.Tenant.SalesAreas.SalesArea>(new object[] { salesArea.Id },
                find => find.IncludeCollection(sa => sa.ChannelGroups).IncludeCollection(sa => sa.Holidays));
            if (entity != null)
            {
                _mapper.Map(salesArea, entity);
                _dbContext.Update(entity, post => post.MapTo(salesArea), _mapper);
                _sequenceRebuilder.Execute(_dbContext, e => e.CustomId);
            }
        }

        public void Update(List<Domain.Shared.SalesAreas.SalesArea> salesAreas)
        {
            var salesAreasWithoutCustomId = salesAreas.Where(e => e.CustomId == 0);

            var newIdentities = new Stack<int>(
                _identityGenerator.GetIdentities<SalesAreaNoIdentity>(
                    salesAreasWithoutCustomId.Count()
                ).Select(e => e.Id)
            );

            foreach (var salesArea in salesAreasWithoutCustomId)
            {
                salesArea.CustomId = newIdentities.Pop();
            }

            using (var transaction = _dbContext.Specific.Database.BeginTransaction())
            {
                _dbContext.BulkInsertEngine.BulkInsertOrUpdate(
                    _mapper.Map<List<Entities.Tenant.SalesAreas.SalesArea>>(salesAreas),
                    new BulkInsertOptions { SetOutputIdentity = true });
                ReplaceChannelGroups(salesAreas);
                ReplaceHolidays(salesAreas);
                SaveChanges();
                transaction.Commit();
            }
        }

        private void ReplaceChannelGroups(List<Domain.Shared.SalesAreas.SalesArea> salesAreas)
        {
            var ids = salesAreas.Select(x => x.Id);
            var existingChannelGroups = _dbContext.Query<Entities.Tenant.SalesAreas.SalesAreasChannelGroup>()
                                      .Where(x => ids.Contains(x.SalesAreaId)).ToArray();
            _dbContext.RemoveRange(existingChannelGroups);

            var salesAreasEntities = _mapper.Map<Entities.Tenant.SalesAreas.SalesArea[]>(salesAreas);
            var newChannelsGroups = salesAreasEntities.SelectMany(x => x.ChannelGroups.Select(s => s)).ToList();
            _dbContext.BulkInsertEngine.BulkInsert(newChannelsGroups,
                new BulkInsertOptions { PreserveInsertOrder = true, SetOutputIdentity = true });
        }

        private void ReplaceHolidays(List<Domain.Shared.SalesAreas.SalesArea> salesAreas)
        {
            var ids = salesAreas.Select(x => x.Id);
            var existingHolidays = _dbContext.Query<Entities.Tenant.SalesAreas.SalesAreasHoliday>()
                                                      .Where(x => ids.Contains(x.SalesAreaId)).ToArray();

            var salesAreasEntities = _mapper.Map<Entities.Tenant.SalesAreas.SalesArea[]>(salesAreas);
            var newHolidays = salesAreasEntities.SelectMany(salesArea => salesArea.Holidays.Select(s => s)).ToArray();

            var holidaysToDelete = existingHolidays.Where(x => !newHolidays.Any(y => y.Start == x.Start &&
                        y.End == x.End && y.SalesAreaId == x.SalesAreaId && y.Type == x.Type)).ToArray();
            _dbContext.RemoveRange(holidaysToDelete);

            var holidaysToInsert = newHolidays.Where(x => !existingHolidays.Any(y => y.Start == x.Start &&
                        y.End == x.End && y.SalesAreaId == x.SalesAreaId && y.Type == x.Type)).ToList();
            _dbContext.BulkInsertEngine.BulkInsert(holidaysToInsert,
                new BulkInsertOptions { PreserveInsertOrder = true, SetOutputIdentity = true });
        }
    }
}
