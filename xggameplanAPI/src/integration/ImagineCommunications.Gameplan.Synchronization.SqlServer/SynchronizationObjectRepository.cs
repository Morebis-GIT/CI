using System;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using ImagineCommunications.Gameplan.Synchronization.Exceptions;
using ImagineCommunications.Gameplan.Synchronization.Interfaces;
using ImagineCommunications.Gameplan.Synchronization.SqlServer.Context;
using ImagineCommunications.Gameplan.Synchronization.SqlServer.Entities;

namespace ImagineCommunications.Gameplan.Synchronization.SqlServer
{
    public class SynchronizationObjectRepository : ISynchronizationObjectRepository
    {
        private readonly SynchronizationDbContext _dbContext;
        private readonly IMapper _mapper;

        public SynchronizationObjectRepository(SynchronizationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper;
        }

        public Objects.SynchronizationObject GetLastCreated()
        {
            var synchronizationObject = _dbContext.Objects
                .Include(e => e.Owners)
                .ProjectTo<Objects.SynchronizationObject>(_mapper.ConfigurationProvider)
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefault();
            if (synchronizationObject == null)
            {
                throw new InvalidOperationException("Current synchronization object not found");
            }

            return synchronizationObject;
        }

        public Objects.SynchronizationObject GetById(Guid id, Guid ownerObjectId)
        {
            var synchronizationObject = _dbContext.Objects
                .Include(e => e.Owners)
                .ProjectTo<Objects.SynchronizationObject>(_mapper.ConfigurationProvider)
                .SingleOrDefault(x => x.Id == id);
            var owner = synchronizationObject?.Owners.SingleOrDefault(o => o.Id == ownerObjectId);
            if (owner == null)
            {
                return null;
            }

            return synchronizationObject;
        }

        public Objects.SynchronizationObject GetActiveByOwnerId(string ownerId)
        {
            var synchronizationObject = GetLastCreated();
            var owner = synchronizationObject.Owners.SingleOrDefault(oo => oo.OwnerId == ownerId && oo.IsActive);
            if (owner == null)
            {
                return null;
            }

            return synchronizationObject;
        }

        public Objects.SynchronizationObject Capture(Guid synchronizationObjectId, string ownerId, int? serviceId = null)
        {
            var synchronizationObject = GetById(synchronizationObjectId);

            synchronizationObject.OwnerCount++;
            if (serviceId.HasValue)
            {
                synchronizationObject.ServiceId = serviceId.Value;
            }

            _dbContext.Update(synchronizationObject);
            _dbContext.Add(new SynchronizationObjectOwner
            {
                SynchronizationObjectId = synchronizationObject.Id,
                OwnerId = ownerId,
                IsActive = true,
                CapturedDate = DateTime.UtcNow
            });

            var result = _mapper.Map<Objects.SynchronizationObject>(synchronizationObject);

            return result;
        }

        public void Release(Guid synchronizationObjectId, string ownerId)
        {
            var synchronizationObject = GetById(synchronizationObjectId);
            var owner = synchronizationObject.Owners.Single(oo =>
                oo.SynchronizationObjectId == synchronizationObjectId &&
                oo.OwnerId == ownerId &&
                oo.IsActive);

            synchronizationObject.OwnerCount--;
            _dbContext.Update(synchronizationObject);

            if (synchronizationObject.OwnerCount == 0)
            {
                _dbContext.Add(new SynchronizationObject());
            }

            owner.IsActive = false;
            owner.ReleasedDate = DateTime.UtcNow;
            _dbContext.Update(owner);
        }

        public void SaveChanges()
        {
            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new DbConcurrencyException("Concurrency error", e);
            }
        }

        public void DiscardChanges()
        {
            _dbContext.ChangeTracker.Entries()
                .Where(e =>
                    e.State == EntityState.Added ||
                    e.State == EntityState.Modified ||
                    e.State == EntityState.Deleted ||
                    e.State == EntityState.Unchanged)
                .ToList().ForEach(x => x.State = EntityState.Detached);
        }

        private SynchronizationObject GetById(Guid synchronizationObjectId)
        {
            var synchronizationObject = _dbContext.Objects.Find(synchronizationObjectId);
            if (synchronizationObject == null)
            {
                throw new InvalidOperationException($"Synchronization object not found. SynchronizationObject.Id = '{synchronizationObjectId}'");
            }

            var entry = _dbContext.Entry(synchronizationObject);
            var collection = entry.Collection<SynchronizationObjectOwner>(o => o.Owners);
            collection.Load();

            return synchronizationObject;
        }
    }
}
