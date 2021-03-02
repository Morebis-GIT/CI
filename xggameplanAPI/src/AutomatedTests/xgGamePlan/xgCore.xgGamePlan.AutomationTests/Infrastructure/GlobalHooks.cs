using System;
using System.Globalization;
using System.Linq;
using AutoFixture;
using BoDi;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints;
using xgCore.xgGamePlan.AutomationTests.Extensions;

namespace xgCore.xgGamePlan.AutomationTests.Infrastructure
{
    [Binding]
    public class GlobalHooks
    {
        [BeforeTestRun]
        private static void BeforeTestRun(IObjectContainer objectContainer)
        {
            objectContainer.RegisterInstanceAs(new ConfigurationReader("config.json"));
            objectContainer.RegisterApiEndpoints(ApiEndpointsHelper.CollectApiEndpointInterfaces().ToArray());
        }

        [BeforeScenario]
        private static void BeforeScenario(IObjectContainer objectContainer)
        {
            objectContainer.RegisterFactoryAs<IFixture>(oc =>
            {
                var fixture = new Fixture();
                fixture.Customizations.Add(new StringGenerator(() =>
                    Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture)));
                return fixture;
            });
        }
    }
}
