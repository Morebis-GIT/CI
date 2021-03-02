using AutoFixture;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints;

namespace xgCore.xgGamePlan.AutomationTests.Features
{
    public abstract class BaseSteps<T>
    {
        public BaseSteps(ScenarioContext context)
        {
            ScenarioContext = context;
        }

        protected ConfigurationReader ConfigReader => ScenarioContext.ScenarioContainer.Resolve<ConfigurationReader>();

        protected ScenarioContext ScenarioContext { get; }

        protected IFixture Fixture => ScenarioContext.ScenarioContainer.Resolve<IFixture>();

        protected T Api => ScenarioContext.ScenarioContainer.Resolve<T>();
    }
}
