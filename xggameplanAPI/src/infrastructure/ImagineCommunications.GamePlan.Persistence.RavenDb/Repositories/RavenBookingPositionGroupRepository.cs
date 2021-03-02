using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositionGroups;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositionGroups.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Linq;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenBookingPositionGroupRepository : IBookingPositionGroupRepository, IDisposable
    {
        private readonly IDocumentSession _session;

        public RavenBookingPositionGroupRepository(IDocumentSession session)
        {
            _session = session;
        }

        public void Add(BookingPositionGroup positionGroup)
        {
            lock (_session)
            {
                _session.Store(positionGroup);
            }
        }

        public void AddRange(IEnumerable<BookingPositionGroup> positionGroups)
        {
            lock (_session)
            {
                var options = new BulkInsertOptions {OverwriteExisting = true};
                using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert(null, options))
                {
                    positionGroups.ToList().ForEach(item => bulkInsert.Store(item));
                }
            }
        }

        public void Delete(int id)
        {
            lock (_session)
            {
                var positionGroup = Get(id);
                if (positionGroup is null)
                {
                    return;
                }

                _session.Delete(positionGroup);
            }
        }

        public void DeleteRangeByGroupId(IEnumerable<int> ids)
        {
            lock (_session)
            {
                var bookingPositionGroups = _session.GetAll<BookingPositionGroup>(s => s.Id.In(ids.ToList()));
                foreach (var positionGroup in bookingPositionGroups)
                {
                    _session.Delete(positionGroup);
                }
            }
        }

        public BookingPositionGroup Get(int id) => _session.Load<BookingPositionGroup>(id);

        public IEnumerable<BookingPositionGroup> GetAll() => _session.GetAll<BookingPositionGroup>();

        public IEnumerable<BookingPositionGroup> GetByGroupIds(IEnumerable<int> groupIds) =>
            _session.GetAll<BookingPositionGroup>(g => g.GroupId.In(groupIds));

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }

        public void Update(BookingPositionGroup positionGroup)
        {
            lock (_session)
            {
                _session.Store(positionGroup);
            }
        }

        public void Dispose()
        {
            _session?.Dispose();
        }
    }
}
