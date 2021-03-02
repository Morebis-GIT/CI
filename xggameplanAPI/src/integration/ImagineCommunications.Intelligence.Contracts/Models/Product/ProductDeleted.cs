using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Product;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Product
{
    public class ProductDeleted : IProductDeleted
    {
        public string Externalidentifier { get; }

        public ProductDeleted(string externalidentifier)
        {
            Externalidentifier = externalidentifier;
        }
    }
}
