using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using xggameplan.Filters;
using xggameplan.TestEnv;

namespace xggameplan.Modules
{
    public class TestEnvAutofacModule : Module
    {
        private readonly IConfiguration _configuration;
        public TestEnvAutofacModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        protected override void Load(ContainerBuilder builder)
        {
            var testEnvironment = new TestEnvironment(
              _configuration,
              new TestEnvironmentFeature(TestEnvironmentFeatures.AutomationTests,
                  TestEnvironmentOptions.WaitForIndexes | TestEnvironmentOptions.ExceptionFilter |
                  TestEnvironmentOptions.AutoBookStub),
              new TestEnvironmentFeature(TestEnvironmentFeatures.PerformanceTests,
                  TestEnvironmentOptions.ExceptionFilter | TestEnvironmentOptions.PerformanceLog |
                  TestEnvironmentOptions.AutoBookStub));

            builder.RegisterInstance(testEnvironment).As<ITestEnvironment>();

            if (!testEnvironment.Enabled)
            {
                return;
            }

            builder.RegisterType<NullLogger<PerformanceLog>>()
                    .As<ILogger<PerformanceLog>>()
                    .InstancePerLifetimeScope();

            //test environment filters should be registered first in order to be executed last after all db management filters
            if (testEnvironment.HasOptions(TestEnvironmentOptions.PerformanceLog))
            {
                builder
                    .RegisterType<TestEnvironmentPerformanceLogApiFilter>()
                    .AsWebApiActionFilterFor<ApiController>()
                    .InstancePerRequest();
            }

            if (testEnvironment.HasOptions(TestEnvironmentOptions.ExceptionFilter))
            {
                builder
                    .RegisterType<TestEnvironmentExceptionFilter>()
                    .AsWebApiExceptionFilterOverrideFor<ApiController>()
                    .InstancePerRequest();
            }

            if (testEnvironment.HasOptions(TestEnvironmentOptions.WaitForIndexes))
            {
                builder
                    .RegisterType<TestEnvironmentWaitForIndexesApiFilter>()
                    .AsWebApiActionFilterFor<ApiController>()
                    .InstancePerRequest();
            }
        }
    }
}
