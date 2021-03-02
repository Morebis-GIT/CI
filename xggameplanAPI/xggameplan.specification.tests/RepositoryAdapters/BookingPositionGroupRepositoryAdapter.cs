using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositionGroups;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositionGroups.Objects;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class BookingPositionGroupRepositoryAdapter : RepositoryTestAdapter<BookingPositionGroup, IBookingPositionGroupRepository, int>
    {
        public BookingPositionGroupRepositoryAdapter(IScenarioDbContext dbContext, IBookingPositionGroupRepository repository) : base(dbContext,
            repository)
        {
        }

        protected override BookingPositionGroup Add(BookingPositionGroup model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<BookingPositionGroup> AddRange(params BookingPositionGroup[] models) => throw new NotImplementedException();

        protected override int Count() => throw new NotImplementedException();

        protected override void Delete(int id) => Repository.Delete(id);

        protected override IEnumerable<BookingPositionGroup> GetAll() => Repository.GetAll();

        protected override BookingPositionGroup GetById(int id) => Repository.Get(id);

        protected override void Truncate() => throw new NotImplementedException();

        protected override BookingPositionGroup Update(BookingPositionGroup model)
        {
            Repository.Update(model);
            return model;
        }
    }
}
