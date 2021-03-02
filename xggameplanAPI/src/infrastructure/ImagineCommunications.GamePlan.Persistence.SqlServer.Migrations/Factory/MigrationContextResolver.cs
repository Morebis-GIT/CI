using System;
using System.Linq;
using System.Reflection;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.DbContext;
using Microsoft.Extensions.Configuration;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.Factory
{
    public class MigrationContextResolver
    {
        private readonly IConfiguration _configuration;

        public MigrationContextResolver(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public IMigrationDbContext ResolveDbContext(CommandLineOptions options)
        {
            if (string.IsNullOrEmpty(options.DbContext))
            {
                var tenantContextFactory = new TenantMigrationContextFactory(_configuration);

                return !string.IsNullOrEmpty(options.ConnectionString) ?
                    tenantContextFactory.CreateDbContext(options.ConnectionString) :
                    tenantContextFactory.CreateDbContext();
            }

            var contextType = Assembly.GetExecutingAssembly().GetTypes()
                    .FirstOrDefault(x => typeof(IMigrationDbContext).IsAssignableFrom(x) &&
                    x.Name.ToLowerInvariant() == options.DbContext.ToLowerInvariant()) ??
                    throw new Exception($"Migration context with name {options.DbContext} doesn't exist");

            var contextFactoryGenericType = typeof(IMigrationDbContextFactory<>).MakeGenericType(contextType);

            var contextFactoryType = Assembly.GetExecutingAssembly().GetTypes()
                    .FirstOrDefault(x => contextFactoryGenericType.IsAssignableFrom(x)) ??
                throw new Exception($"Unable to resolve factory for context with name {options.DbContext}");

            var contextFactoryInstance = Activator.CreateInstance(contextFactoryType, _configuration);

            MethodInfo creationMethod;
            object[] arguments;

            if (string.IsNullOrEmpty(options.ConnectionString))
            {
                creationMethod = contextFactoryType.GetMethod("CreateDbContext",
                    new Type[0]);

                arguments = null;
            }
            else
            {
                creationMethod = contextFactoryType.GetMethod("CreateDbContext",
                    new Type[] { typeof(string) });

                arguments = new object[] {options.ConnectionString};
            }

            return (IMigrationDbContext) creationMethod.Invoke(contextFactoryInstance, arguments);
        }
    }
}
