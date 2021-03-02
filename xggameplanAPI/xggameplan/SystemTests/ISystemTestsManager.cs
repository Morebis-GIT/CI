using System.Collections.Generic;

namespace xggameplan.SystemTests
{
    public interface ISystemTestsManager
    {
        List<SystemTestResult> ExecuteTests(SystemTestCategories systemTestCategory);
    }
}
