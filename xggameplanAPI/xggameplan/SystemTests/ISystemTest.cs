using System.Collections.Generic;

namespace xggameplan.SystemTests
{
    /// <summary>
    /// Interface for running a system test
    /// </summary>
    public interface ISystemTest
    {
        List<SystemTestResult> Execute(SystemTestCategories systemTestCategory);

        /// <summary>
        /// Determines whether tests can be executed for category.
        /// </summary>
        /// <param name="systemTestCategories"></param>
        /// <returns></returns>
        bool CanExecute(SystemTestCategories systemTestCategories);
    }
}
