using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositionGroups;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositionGroups.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using BookingPositionGroupEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BookingPositionGroups.BookingPositionGroup;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class BookingPositionGroupRepository : IBookingPositionGroupRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public BookingPositionGroupRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void Add(BookingPositionGroup bookingPositionGroup) =>
            _dbContext.Add(_mapper.Map<BookingPositionGroupEntity>(bookingPositionGroup),
                post => post.MapTo(bookingPositionGroup), _mapper);

        public void AddRange(IEnumerable<BookingPositionGroup> bookingPositionGroups) =>
            _dbContext.AddRange(_mapper.Map<BookingPositionGroupEntity[]>(bookingPositionGroups),
                post => post.MapToCollection(bookingPositionGroups), _mapper);

        public void Delete(int id)
        {
            var entity = _dbContext.Find<BookingPositionGroupEntity>(id);
            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public void DeleteRangeByGroupId(IEnumerable<int> ids)
        {
            var bookingPositionGroups = _dbContext.Query<BookingPositionGroupEntity>().Where(x => ids.Contains(x.GroupId)).ToArray();
            if (bookingPositionGroups.Any())
            {
                _dbContext.RemoveRange(bookingPositionGroups);
            }
        }

        public BookingPositionGroup Get(int id) =>
            _dbContext.Query<BookingPositionGroupEntity>()
                .ProjectTo<BookingPositionGroup>(_mapper.ConfigurationProvider)
                .FirstOrDefault(x => x.Id == id);

        public IEnumerable<BookingPositionGroup> GetAll() =>
            _dbContext.Query<BookingPositionGroupEntity>()
                .ProjectTo<BookingPositionGroup>(_mapper.ConfigurationProvider)
                .ToList();

        public IEnumerable<BookingPositionGroup> GetByGroupIds(IEnumerable<int> groupIds) =>
            _dbContext.Query<BookingPositionGroupEntity>()
                .Where(x => groupIds.Any(i => i == x.GroupId))
                .ProjectTo<BookingPositionGroup>(_mapper.ConfigurationProvider)
                .ToList();

        public void SaveChanges() => _dbContext.SaveChanges();

        public void Update(BookingPositionGroup bookingPositionGroup)
        {
            var entity = _dbContext.Query<BookingPositionGroupEntity>()
                .Include(g => g.PositionGroupAssociations)
                .FirstOrDefault(x => x.Id == bookingPositionGroup.Id);

            if (entity != null)
            {
                _mapper.Map(bookingPositionGroup, entity);
                _dbContext.Update(entity, post => post.MapTo(bookingPositionGroup), _mapper);
            }
        }
    }
}
