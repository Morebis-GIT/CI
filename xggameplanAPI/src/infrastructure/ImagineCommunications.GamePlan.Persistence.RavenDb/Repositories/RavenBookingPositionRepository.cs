using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositions;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;
using Raven.Client.Linq;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenBookingPositionRepository : IBookingPositionRepository, IDisposable
    {
        private readonly IDocumentSession _session;

        public RavenBookingPositionRepository(IDocumentSession session)
        {
            _session = session;
        }

        public BookingPosition Get(int id) => _session.Load<BookingPosition>(id);

        public IEnumerable<BookingPosition> GetAll() => _session.GetAll<BookingPosition>();

        public BookingPosition GetByPosition(int position) =>
            _session.Query<BookingPosition>().FirstOrDefault(p => p.Position == position);

        public IEnumerable<BookingPosition> GetByPositions(IEnumerable<int> positions) => _session.GetAll<BookingPosition>(d => d.Position.In(positions));

        public void Dispose()
        {
            _session?.Dispose();
        }
    }
}
