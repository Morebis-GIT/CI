using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using BookingPositionEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BookingPositionGroups.BookingPosition;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class BookingPositionRepository : IBookingPositionRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public BookingPositionRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public BookingPosition Get(int id) => _mapper.Map<BookingPosition>(_dbContext.Find<BookingPositionEntity>(id));

        public IEnumerable<BookingPosition> GetAll() =>
            _dbContext.Query<BookingPositionEntity>()
                .ProjectTo<BookingPosition>(_mapper.ConfigurationProvider)
                .ToList();

        public BookingPosition GetByPosition(int position) =>
            _dbContext.Query<BookingPositionEntity>()
                .ProjectTo<BookingPosition>(_mapper.ConfigurationProvider)
                .FirstOrDefault(x => x.Position == position);

        public IEnumerable<BookingPosition> GetByPositions(IEnumerable<int> positions) =>
            _dbContext.Query<BookingPositionEntity>()
                .Where(x => positions.Contains(x.Position))
                .ProjectTo<BookingPosition>(_mapper.ConfigurationProvider)
                .ToList();
    }
}
