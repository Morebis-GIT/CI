using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;
using System.Web.Http.ValueProviders.Providers;

namespace xggameplan.Helpers
{
    internal sealed class CommaDelimitedToIEnumerableStringParameterBinder : HttpParameterBinding, IValueProviderParameterBinding
    {
        public CommaDelimitedToIEnumerableStringParameterBinder(HttpParameterDescriptor desc)
            : base(desc)
        {
        }

        public override Task ExecuteBindingAsync(ModelMetadataProvider metadataProvider,
                                                 HttpActionContext actionContext,
                                                 CancellationToken cancellationToken)
        {
            var paramValues = actionContext.ControllerContext.Request.RequestUri.ParseQueryString().Get(Descriptor.ParameterName);
            var paramValueAsArray = paramValues?.Split(',').Where(a => !String.IsNullOrWhiteSpace(a)).ToArray();
            SetValue(actionContext, paramValueAsArray);
            return Task.CompletedTask;
        }

        public IEnumerable<ValueProviderFactory> ValueProviderFactories { get; } = new[] { new QueryStringValueProviderFactory() };
    }
}
