using System.Collections.Generic;
using xggameplan.Model;

namespace xggameplan.Services
{
    /// <summary>
    /// Change actions
    /// </summary>
    public enum ChangeActions : byte
    {
        Insert = 0,
        Update = 1,
        Delete = 2
    }


    /// <summary>
    /// Targets of change
    /// </summary>
    public enum ChangeTargets : byte
    {
        SpecificItems = 0,
        AllItems = 1
    }

    /// <summary>
    /// Validation for data change
    /// </summary>
    public interface IDataChangeValidator
    {
        /// <summary>
        /// Validates change
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="changeAction"></param>
        /// <param name="changeTargets"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        IEnumerable<ChangeValidationResult> ValidateChange<T>(ChangeActions changeAction, ChangeTargets changeTargets, IEnumerable<T> targets);

        /// <summary>
        /// Throws exception if validation results contains any errors
        /// </summary>
        /// <param name="results"></param>
        void ThrowExceptionIfAnyErrors(IEnumerable<ChangeValidationResult> results);
    }

    
}
