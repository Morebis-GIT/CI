using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using ImagineCommunications.GamePlan.Domain.Spots;
using xggameplan.Model;

namespace xggameplan.Services
{
    /// <summary>
    /// Validates data changes. This is necessary to prevent certain data changes from happening.
    /// 
    /// Currently we only validate Delete All changes. Ideally, when the run starts then we should log every entity that the run depends on and
    /// prevent any modifications, either by a batch delete or individual modifications.
    /// </summary>
    public class DataChangeValidator : IDataChangeValidator
    {
        private IRunRepository _runRepository;

        public DataChangeValidator(IRunRepository runRepository)
        {
            _runRepository = runRepository;
        }

        /// <summary>
        /// Validates before changing the data.              
        /// </summary>
        /// <param name="changeAction"></param>
        /// <param name="changeTargets"></param>
        /// <param name="targets">Items that will be modified (Empty is target=AllItems)</param>
        /// <returns></returns>
        public IEnumerable<ChangeValidationResult> ValidateChange<T>(ChangeActions changeAction, ChangeTargets changeTargets, IEnumerable<T> targets)
        {
            List<ChangeValidationResult> results = new List<ChangeValidationResult>();

            if (!IsChangeAllowedWhileRunScheduledOrActive<T>(changeAction, changeTargets, targets))
            {                
                // Change to this data is not allowed while run scheduled or active, check if any runs
                var runs = _runRepository.GetAll().Where(r => r.Scenarios.Where(s => s.IsScheduledOrRunning).Any()).ToList();                
                foreach(var run in runs)
                {
                    results.Add(new ChangeValidationResult(ChangeValidationResult.ResultTypes.Error, run, string.Format("Cannot delete data because run {0} is scheduled or active", run.Description)));
                }                                    
            }
            return results;
        }

        /// <summary>
        /// Throws exception if any results
        /// </summary>
        /// <param name="results"></param>
        public void ThrowExceptionIfAnyErrors(IEnumerable<ChangeValidationResult> results)
        {
            ChangeValidationResult result = results.ToList().Where(r => r.ResultType == ChangeValidationResult.ResultTypes.Error).FirstOrDefault();
            if (result != null)
            {                
                throw new Exception(result.Message);
            }
        }

        /// <summary>
        /// Returns whether change is allowed while run is scheduled or active. Currently only Delete All is not allowed.
        /// </summary>
        /// <param name="changeAction"></param>
        /// <param name="changeTargets"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        private bool IsChangeAllowedWhileRunScheduledOrActive<T>(ChangeActions changeAction, ChangeTargets changeTargets, IEnumerable<T> targets)
        {
            if (changeAction == ChangeActions.Delete && changeTargets == ChangeTargets.AllItems)
            {
                // All of the types where repository supports Delete All
                return (Array.IndexOf(new Type[] { typeof(Break), typeof(Campaign), typeof(Clash), typeof(Demographic), typeof(Product), typeof(Programme), typeof(Rating),
                                         typeof(RatingsPredictionSchedule), typeof(Schedule), typeof(Spot), typeof(Universe) }, typeof(T)) == -1);
            }
            return true;     // Allowed for anything else
        }
    }
}
