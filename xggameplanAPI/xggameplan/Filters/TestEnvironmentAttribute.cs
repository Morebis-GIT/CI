using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using xggameplan.TestEnv;

namespace xggameplan.Filters
{
    public class TestEnvironmentAttribute : ActionFilterAttribute
    {
        private readonly TestEnvironmentOptions _options;
        private readonly string[] _features;

        public TestEnvironmentAttribute(TestEnvironmentOptions options, params string[] features)
        {
            _options = options;
            _features = features ?? Array.Empty<string>();
        }

        public TestEnvironmentAttribute(params string[] aliases) :
            this(TestEnvironmentOptions.None, aliases)
        {
        }


        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var testEnvironment = actionContext.ControllerContext.Configuration.DependencyResolver.GetService(
                typeof(ITestEnvironment)) as ITestEnvironment;
            if (!(testEnvironment.Enabled &&
                (testEnvironment.HasOptions(_options) || _features.Any(f => testEnvironment.HasFeature(f)))))
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent("The endpoint is available within test environment only.")
                };
            }
        }
    }
}
