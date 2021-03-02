using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.SpotPlacements;
using xggameplan.AuditEvents;

namespace xggameplan.SystemTasks
{
    /// <summary>
    /// Task to clean up Smooth data
    /// </summary>
    internal class SmoothDataTask : ISystemTask
    {
        private IAuditEventRepository _auditEventRepository;
        private IRepositoryFactory _repositoryFactory;
        private TimeSpan _spotPlacementRetention;

        public SmoothDataTask(IAuditEventRepository auditEventRepository, IRepositoryFactory repositoryFactory, TimeSpan spotPlacementRetention)
        {
            _auditEventRepository = auditEventRepository;
            _repositoryFactory = repositoryFactory;
            _spotPlacementRetention = spotPlacementRetention;
        }

        public string Id
        {
            get { return "SmoothDataTask"; }
        }

        public List<SystemTaskResult> Execute()
        {
            List<SystemTaskResult> results = new List<SystemTaskResult>();

            // Delete SpotPlacements
            try
            {
                results.AddRange(DeleteSpotPlacements(_spotPlacementRetention));     // Hard-coded for the moment
            }
            catch (System.Exception exception)
            {
                results.Add(new SystemTaskResult(SystemTaskResult.ResultTypes.Error, this.Id, string.Format("Error deleting spot placements: {0}", exception.Message)));
            }
            return results;
        }

        /// <summary>
        /// Deletes SpotPlacement docs for dead spots, not modified for more than N days
        /// </summary>
        /// <returns></returns>
        private List<SystemTaskResult> DeleteSpotPlacements(TimeSpan spotPlacementRetention)
        {
            List<SystemTaskResult> results = new List<SystemTaskResult>();
            if (spotPlacementRetention.Ticks != 0)
            {
                try
                {                    
                    using (var scope = _repositoryFactory.BeginRepositoryScope())
                    {
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, string.Format("Deleting spot placements (Days Old={0})", spotPlacementRetention.TotalDays)));
                        var spotPlacementRepository = scope.CreateRepository<ISpotPlacementRepository>();

                        DateTime modifiedBefore = DateTime.UtcNow.Subtract(spotPlacementRetention);
                        spotPlacementRepository.DeleteBefore(modifiedBefore);
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, "Deleted spot placements"));
                    }
                }
                catch (System.Exception exception)
                {
                    results.Add(new SystemTaskResult(SystemTaskResult.ResultTypes.Error, this.Id, string.Format("Error deleting spot placements: {0}", exception.Message)));
                }
            }
            return results;
        }
    }
}
