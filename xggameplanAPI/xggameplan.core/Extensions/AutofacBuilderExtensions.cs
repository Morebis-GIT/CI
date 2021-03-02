using System.Net.Http;
using System.Reflection;
using Autofac;
using Autofac.Core;
using AutoMapper;
using xggameplan.core.Helpers;
using xggameplan.core.Interfaces;

namespace xggameplan.core.Extensions
{
    public static class AutofacBuilderExtensions
    {
        public static ContainerBuilder AddAutoMapper(this ContainerBuilder builder, params Assembly[] assemblies)
        {
            if (builder is null)
            {
                return null;
            }

            var configurationProvider = new MapperConfiguration(config => config.AddMaps(assemblies));

            _ = builder.RegisterInstance(configurationProvider).As<IConfigurationProvider>();
            _ = builder.Register<IMapper>(ctx => new Mapper(ctx.Resolve<IConfigurationProvider>())).InstancePerLifetimeScope();

            return builder;
        }

        /// <summary>
        /// Creates <see cref="IHttpClientServiceBuilder"/> builder instance.
        /// </summary>
        /// <param name="builder">Container builder.</param>
        /// <returns>A <see cref="IHttpClientServiceBuilder"/> that can be used to configure the <see cref="HttpClient"/> client.</returns>
        public static IHttpClientServiceBuilder AddHttpClient<TImplementer, TService>(this ContainerBuilder builder)
        {
            return new HttpClientServiceBuilder(RegisterClient);

            void RegisterClient(HttpClient client)
            {
                string key = typeof(TImplementer).FullName;
                _ = builder.RegisterInstance(client).Named<HttpClient>(key).SingleInstance();

                _ = builder.RegisterType<TImplementer>()
                    .WithParameter(new ResolvedParameter(
                        (pi, ctx) => pi.ParameterType == typeof(HttpClient),
                        (pi, ctx) => ctx.ResolveNamed<HttpClient>(key)))
                    .As<TService>()
                    .SingleInstance();
            }
        }
    }
}
