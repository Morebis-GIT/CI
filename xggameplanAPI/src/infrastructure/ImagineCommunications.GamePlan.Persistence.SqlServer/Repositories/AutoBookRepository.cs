using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping;
using Microsoft.EntityFrameworkCore;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class AutoBookRepository : IAutoBookRepository
    {       
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public AutoBookRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void Add(AutoBook autoBook) =>
            _dbContext.Add(_mapper.Map<Entities.Tenant.AutoBookApi.AutoBook>(autoBook), post => post.MapTo(autoBook), _mapper);

        public void Update(AutoBook autoBook)
        {
            var autoBookId = AutoBookProfile.AutoBookCollectionIdToEntityAutoBookId(autoBook.Id);

            var entity = _dbContext.Query<Entities.Tenant.AutoBookApi.AutoBook>()
                .Include(x => x.Task)
                .FirstOrDefault(x => x.AutoBookId == autoBookId);

            if (entity != null)
            {
                _mapper.Map(autoBook, entity);
                _dbContext.Update(entity, post => post.MapTo(autoBook), _mapper);
            }
        }

        public void Delete(string id)
        {
            var autoBookId = AutoBookProfile.AutoBookCollectionIdToEntityAutoBookId(id);

            var entity = _dbContext.Query<Entities.Tenant.AutoBookApi.AutoBook>()
                .Include(x => x.Task)
                .FirstOrDefault(x => x.AutoBookId == autoBookId);

            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public AutoBook Get(string id)
        {
            var autoBookId = AutoBookProfile.AutoBookCollectionIdToEntityAutoBookId(id);

            return _dbContext.Query<Entities.Tenant.AutoBookApi.AutoBook>()
                .Where(x => x.AutoBookId == autoBookId).ProjectTo<AutoBook>(_mapper.ConfigurationProvider).FirstOrDefault();
        }

        public IEnumerable<AutoBook> GetAll() => _dbContext
            .Query<Entities.Tenant.AutoBookApi.AutoBook>()
            .ProjectTo<AutoBook>(_mapper.ConfigurationProvider)
            .ToList();

        public int CountAll => _dbContext.Query<Entities.Tenant.AutoBookApi.AutoBook>().Count();

        public void SaveChanges() => _dbContext.SaveChanges();
    }
}
