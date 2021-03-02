using xggameplan.core.TestEnvironment;

namespace xggameplan.core.Interfaces
{
    public interface ITestEnvironmentDataService
    {
        TestEnvironmentRunResult PopulateRunResult(TestEnvironmentOutputFilesInfo outputFilesResult);

        int PopulateSmoothConfiguration();
    }
}
