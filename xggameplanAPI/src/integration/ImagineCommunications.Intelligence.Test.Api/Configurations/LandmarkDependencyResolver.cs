using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using Microsoft.Extensions.DependencyInjection;

namespace ImagineCommunications.Intelligence.Test.Api
{
    public class LandmarkDependencyResolver : IDependencyResolver
    {
        protected IServiceProvider ServiceProvider;

        public LandmarkDependencyResolver(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

        public IDependencyScope BeginScope() => new LandmarkDependencyResolver(ServiceProvider.CreateScope().ServiceProvider);

        public object GetService(Type serviceType) => ServiceProvider.GetService(serviceType);

        public IEnumerable<object> GetServices(Type serviceType) => ServiceProvider.GetServices(serviceType);

        public void AddService() { }

        public void Dispose() { }
    }

    public static class ServiceProviderExtensions
    {
        public static IServiceCollection AddControllersAsServices(this IServiceCollection services, IEnumerable<Type> serviceTypes)
        {
            foreach (var type in serviceTypes)
            {
                services.AddTransient(type);
            }

            return services;
        }
    }
}
