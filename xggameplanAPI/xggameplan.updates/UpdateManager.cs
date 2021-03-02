using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Maintenance.UpdateDetail;

namespace xggameplan.Updates
{
    /// <summary>
    /// Update manager, applies updates and/or rolls back
    /// </summary>
    public class UpdateManager : IUpdateManager
    {
        private readonly IUpdateDetailsRepository _updateDetailsRepository;
        private readonly UpdateConfiguration _configuration;

        public UpdateManager(
            IUpdateDetailsRepository updateDetailsRepository,
            string masterConnectionString,
            List<string> tenantConnectionStrings,
            string updatesFolderRoot,
            IMapper mapper)
        {
            _updateDetailsRepository = updateDetailsRepository;
            _configuration = new UpdateConfiguration(tenantConnectionStrings, masterConnectionString, updatesFolderRoot, mapper);

            _ = CreateUpdateDirectoryIfNotExists(updatesFolderRoot);
        }

        /// <summary>
        /// Returns whether update has been applied
        /// </summary>
        /// <param name="updateId"></param>
        /// <returns></returns>
        private bool IsApplied(Guid updateId) =>
            _updateDetailsRepository.Find(updateId) != null;

        /// <summary>
        /// Applies update
        /// </summary>
        /// <param name="updateId"></param>
        public void Apply(Guid updateId)
        {
            IUpdate update = GetUpdate(updateId);
            if (update is null)
            {
                throw new Exception($"Update {updateId} is not a valid update");
            }

            if (IsApplied(updateId))
            {
                return;
            }

            var missingUpdateIds = GetMissingDependentUpdates(update);
            if (missingUpdateIds.Count > 0)
            {
                throw new Exception($"Update cannot be applied because it depends on update {missingUpdateIds[0]}");
            }

            try
            {
                update.Apply();

                var updateDetails = new UpdateDetails()
                {
                    Id = update.Id,
                    Name = update.Name,
                    TimeApplied = DateTime.UtcNow
                };

                _updateDetailsRepository.Add(updateDetails);
                _updateDetailsRepository.SaveChanges();
            }
            catch (Exception exception)
            {
                try
                {
                    if (update.SupportsRollback)
                    {
                        update.RollBack();
                    }
                }
                catch (Exception rollBackException)
                {
                    throw new Exception($"Error applying update {update.Name}, roll back failed", rollBackException);
                }

                throw new Exception($"Error applying update {update.Name} - {exception.InnerException?.Message}", exception);
            }
        }

        /// <summary>
        /// Returns missing updates that this update depends on
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        private List<Guid> GetMissingDependentUpdates(IUpdate update)
        {
            // Get all applied updates
            var appliedUpdateIds = _updateDetailsRepository.GetAll().Select(u => u.Id).Distinct();

            // Get updates missing
            var updateIdsMissing = update.DependsOnUpdates.Where(uid => !appliedUpdateIds.Contains(uid)).ToList();

            return updateIdsMissing;
        }

        /// <summary>
        /// Rolls back the update.
        /// </summary>
        /// <param name="updateId"></param>
        public void RollBack(Guid updateId)
        {
            IUpdate update = GetUpdate(updateId);
            if (update is null)
            {
                throw new Exception($"Update {updateId} is not a valid update");
            }

            if (IsApplied(updateId))
            {
                try
                {
                    update.RollBack();

                    _updateDetailsRepository.Remove(updateId);
                    _updateDetailsRepository.SaveChanges();
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error rolling back update {update.Name}", exception);
                }
            }
        }

        /// <summary>
        /// Returns update by ID
        /// </summary>
        /// <param name="updateId"></param>
        /// <returns></returns>
        public IUpdate GetUpdate(Guid updateId) =>
            GetAllUpdates().Find(u => u.Id == updateId);

        /// <summary>
        /// Returns all updates
        /// </summary>
        /// <returns></returns>
        private List<IUpdate> GetAllUpdates()
        {
            var updates = new List<IUpdate>();

            var updateTypes = typeof(UpdateManager).Assembly.GetTypes().Where(IsUpdate);

            foreach (var updateType in updateTypes)
            {
                var update = (IUpdate)Activator.CreateInstance(updateType, _configuration);
                updates.Add(update);
            }

            return updates;
        }

        /// <summary>
        /// Indicates whether the type is an update, supports IUpdate
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsUpdate(Type type)
        {
            if (!type.IsClass)
            {
                return false;
            }

            return type.GetInterfaces().Any(i => i == typeof(IUpdate));
        }

        /// <summary>
        /// Returns all updates needed to update from old database version to
        /// new database version, returns in order that updates need to be applied
        /// </summary>
        /// <param name="oldDatabaseVersion"></param>
        /// <param name="newDatabaseVersion"></param>
        /// <returns></returns>
        public List<Guid> GetUpdates(string oldDatabaseVersion, string newDatabaseVersion)
        {
            List<IUpdate> allUpdates = GetAllUpdates();

            int oldDatabaseVersionOrder = VersionUtilities.GetVersionOrder(oldDatabaseVersion);
            int newDatabaseVersionOrder = VersionUtilities.GetVersionOrder(newDatabaseVersion);

            // Filter all updates to move from (just after) old database version
            // to new version
            List<IUpdate> updatesForVersions = new List<IUpdate>();
            updatesForVersions.AddRange(allUpdates.Where(u => VersionUtilities.GetVersionOrder(u.DatabaseVersion) > oldDatabaseVersionOrder && VersionUtilities.GetVersionOrder(u.DatabaseVersion) <= newDatabaseVersionOrder));

            // Return versions in order
            // TODO: Decide if this order is OK for IUpdate.DependsOnUpdates
            return updatesForVersions.OrderBy(u => VersionUtilities.GetVersionOrder(u.DatabaseVersion)).ToList().Select(u => u.Id).ToList();
        }

        private static bool CreateUpdateDirectoryIfNotExists(string updatesFolderRoot) =>
            Directory.CreateDirectory(updatesFolderRoot) != null;
    }
}
