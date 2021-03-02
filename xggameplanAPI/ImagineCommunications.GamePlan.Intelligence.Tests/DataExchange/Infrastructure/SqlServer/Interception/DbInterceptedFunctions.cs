namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer.Interception
{
    internal class DbInterceptedFunctions
    {
        internal static bool Contains(IFtsInterceptionProvider ftsInjectionProvider, object entity, string propertyName,
            string searchCondition)
        {
            return entity != null &&
                   ftsInjectionProvider.Contains(entity.GetType(), entity, propertyName, searchCondition);
        }

        internal static bool Contains(IFtsInterceptionProvider ftsInjectionProvider, object entity, string propertyName,
            string searchCondition, int languageTerm)
        {
            return Contains(ftsInjectionProvider, entity, propertyName, searchCondition);
        }
    }
}
