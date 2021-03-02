using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.TotalRatings;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using TotalRatingEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.TotalRating;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class TotalRatingRepository : ITotalRatingRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public TotalRatingRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public IEnumerable<TotalRating> GetAll() =>
            _dbContext.Query<TotalRatingEntity>()
                .ProjectTo<TotalRating>(_mapper.ConfigurationProvider)
                .ToList();

        public TotalRating Get(int id) => _mapper.Map<TotalRating>(_dbContext.Find<TotalRatingEntity>(id));

        public void AddRange(IEnumerable<TotalRating> totalRatings) =>
            _dbContext.AddRange(_mapper.Map<TotalRatingEntity[]>(totalRatings),
                post => post.MapToCollection(totalRatings), _mapper);

        public IEnumerable<TotalRating> Search(string salesArea, DateTime startDate, DateTime endDate) =>
            _dbContext.Query<TotalRatingEntity>()
                .Where(c => c.SalesArea == salesArea && c.Date >= startDate && c.Date <= endDate)
                .ProjectTo<TotalRating>(_mapper.ConfigurationProvider)
                .ToList();

        public IEnumerable<TotalRating> SearchByMonths(DateTime startDate, DateTime endDate) =>
            _dbContext.Query<TotalRatingEntity>()
                .Where(c => c.Date.Month >= startDate.Month && c.Date.Month <= endDate.Month)
                .ProjectTo<TotalRating>(_mapper.ConfigurationProvider)
                .ToList();

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
