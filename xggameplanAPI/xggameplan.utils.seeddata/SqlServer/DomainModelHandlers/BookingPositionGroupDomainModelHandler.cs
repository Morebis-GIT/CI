using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositionGroups;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositionGroups.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using BookingPositionGroupEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BookingPositionGroups.BookingPositionGroup;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class BookingPositionGroupDomainModelHandler : IDomainModelHandler<BookingPositionGroup>
    {
        private readonly IBookingPositionGroupRepository _repository;
        private readonly ISqlServerDbContext _dbContext;

        public BookingPositionGroupDomainModelHandler(
            IBookingPositionGroupRepository repository,
            ISqlServerDbContext dbContext)
        {
            _repository = repository;
            _dbContext = dbContext;
        }

        public BookingPositionGroup Add(BookingPositionGroup model)
        {
            _repository.Add(model);
            return model;
        }

        public void AddRange(params BookingPositionGroup[] models)
        {
            foreach (var model in models)
            {
                _repository.Add(model);
            }
        }

        public int Count() => _dbContext.Query<BookingPositionGroupEntity>().Count();

        public void DeleteAll() => _dbContext.Truncate<BookingPositionGroupEntity>();

        public IEnumerable<BookingPositionGroup> GetAll() => _repository.GetAll();
    }
}
