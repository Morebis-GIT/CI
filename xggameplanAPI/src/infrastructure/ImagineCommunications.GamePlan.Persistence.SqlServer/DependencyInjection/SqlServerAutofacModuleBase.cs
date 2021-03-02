using Autofac;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.DependencyInjection
{
    public abstract class SqlServerAutofacModuleBase : Autofac.Module
    {
        public const string DbContextLoggerFactoryRegistrationName = "__dbcontext_internal_logger_factory__";

        protected virtual IClock ResolveClock(IComponentContext context)
        {
            return context.ResolveOptional<IClock>() ?? SystemClock.Instance;
        }

        protected virtual bool ResolveDbContextLoggerFactory(IComponentContext context, out ILoggerFactory loggerFactory)
        {
            loggerFactory = context.ResolveOptionalNamed<ILoggerFactory>(DbContextLoggerFactoryRegistrationName);
            return loggerFactory != null;
        }
    }
}
