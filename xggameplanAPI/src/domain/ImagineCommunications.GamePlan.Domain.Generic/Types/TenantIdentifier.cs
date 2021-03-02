namespace ImagineCommunications.GamePlan.Domain.Generic.Types
{
    public class TenantIdentifier
    {
        public TenantIdentifier(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; }
        public string Name { get; }
    }
}
