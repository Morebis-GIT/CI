using BoDi;
using TechTalk.SpecFlow;

namespace xggameplan.specification.tests.Infrastructure.EnvironmentSettings
{
    [Binding]
    public class EnvironmentSettingsHook
    {
        [BeforeTestRun]
        public static void RegisterEnvironmentSettings(IObjectContainer objectContainer)
        {
            objectContainer.RegisterInstanceAs<IEnvironmentSettings>(new EnvironmentSettings());
        }
    }
}
