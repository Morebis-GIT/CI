using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.TotalRatings;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using Microsoft.EntityFrameworkCore;
using xggameplan.core.Extensions.AutoMapper;
using TotalRatingEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.TotalRating;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class TotalRatingRepository : ITotalRatingRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;
        private readonly IMapper _mapper;

        public TotalRatingRepository(
            ISqlServerTenantDbContext dbContext,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
            _mapper = mapper;
        }

        public IEnumerable<TotalRating> GetAll() =>
            _mapper.Map<List<TotalRating>>(
                _dbContext.Query<TotalRatingEntity>().AsNoTracking(),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public TotalRating Get(int id) => _mapper.Map<TotalRating>(_dbContext.Find<TotalRatingEntity>(id),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public void AddRange(IEnumerable<TotalRating> totalRatings) =>
            _dbContext.AddRange(_mapper.Map<TotalRatingEntity[]>(totalRatings, opts => opts.UseEntityCache(_salesAreaByNameCache)),
                post => post.MapToCollection(totalRatings, opts => opts.UseEntityCache(_salesAreaByIdCache)), _mapper);

        public IEnumerable<TotalRating> Search(string salesArea, DateTime startDate, DateTime endDate) =>
            _mapper.Map<List<TotalRating>>(
                _dbContext.Query<TotalRatingEntity>()
                    .Where(c => c.SalesArea.Name == salesArea && c.Date >= startDate && c.Date <= endDate).AsNoTracking(),
                opts => opts.UseEntityCache(_salesAreaByIdCache)).ToList();

        public IEnumerable<TotalRating> SearchByMonths(DateTime startDate, DateTime endDate) =>
            _mapper.Map<List<TotalRating>>(
                _dbContext.Query<TotalRatingEntity>()
                    .Where(c => c.Date.Month >= startDate.Month && c.Date.Month <= endDate.Month).AsNoTracking(),
                opts => opts.UseEntityCache(_salesAreaByIdCache)).ToList();

        public void DeleteRange(IEnumerable<int> ids)
        {
            var totalRatings = _dbContext.Query<TotalRatingEntity>()
                .Where(x => ids.Contains(x.Id))
                .ToArray();

            if (totalRatings.Any())
            {
                _dbContext.RemoveRange(totalRatings);
            }
        }

        public void SaveChanges() => _dbContext.SaveChanges();

        public void Truncate() => _dbContext.Truncate<TotalRatingEntity>();
    }
}
