namespace ImagineCommunications.GamePlan.Domain.Generic.DbSequence
{
    public interface IIdentityGeneratorResolver
    {
        IIdentityGenerator Resolve<T>() where T : class;
    }
}
