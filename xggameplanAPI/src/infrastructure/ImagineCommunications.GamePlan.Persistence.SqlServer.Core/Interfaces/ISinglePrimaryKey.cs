namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces
{
    public interface ISinglePrimaryKey<TKey>
    {
        TKey Id { get; set; }
    }
}
