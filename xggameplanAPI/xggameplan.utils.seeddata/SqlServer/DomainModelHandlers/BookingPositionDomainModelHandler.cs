using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using BookingPositionEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BookingPositionGroups.BookingPosition;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class BookingPositionDomainModelHandler : IDomainModelHandler<BookingPosition>
    {
        private readonly ISqlServerDbContext _dbContext;
        private readonly IMapper _mapper;

        public BookingPositionDomainModelHandler(ISqlServerDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper;
        }

        public BookingPosition Add(BookingPosition model)
        {
            _ = _dbContext.Add(_mapper.Map<BookingPositionEntity>(model), post => post.MapTo(model), _mapper);
            return model;
        }

        public void AddRange(params BookingPosition[] models) =>
            _dbContext.BulkInsertEngine.BulkInsert(_mapper.Map<List<BookingPositionEntity>>(models),
                post => post.TryToUpdate(models), _mapper);

        public int Count() => _dbContext.Query<BookingPositionEntity>().Count();

        public void DeleteAll() => _dbContext.Truncate<BookingPositionEntity>();

        public IEnumerable<BookingPosition> GetAll() =>
            _dbContext.Query<BookingPositionEntity>().ProjectTo<BookingPosition>(_mapper.ConfigurationProvider).AsEnumerable();
    }
}
