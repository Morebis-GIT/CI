namespace xggameplan.TestEnv
{
    public interface ITestEnvironment
    {
        TestEnvironmentOptions Mode { get; }
        bool Enabled { get; }
        bool HasOptions(TestEnvironmentOptions options);
        bool HasFeature(string featureName);

        void Reload();
    }
}
