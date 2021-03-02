using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositions;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class BookingPositionRepositoryAdapter : RepositoryTestAdapter<BookingPosition, IBookingPositionRepository, int>
    {
        public BookingPositionRepositoryAdapter(IScenarioDbContext dbContext, IBookingPositionRepository repository) : base(dbContext,
            repository)
        {
        }

        protected override BookingPosition Add(BookingPosition model) => throw new NotImplementedException();

        protected override IEnumerable<BookingPosition> AddRange(params BookingPosition[] models) => throw new NotImplementedException();

        protected override int Count() => throw new NotImplementedException();

        protected override void Delete(int id) => throw new NotImplementedException();

        protected override IEnumerable<BookingPosition> GetAll() => Repository.GetAll();

        protected override BookingPosition GetById(int id) => Repository.Get(id);

        protected override void Truncate() => throw new NotImplementedException();

        protected override BookingPosition Update(BookingPosition model) => throw new NotImplementedException();
    }
}
