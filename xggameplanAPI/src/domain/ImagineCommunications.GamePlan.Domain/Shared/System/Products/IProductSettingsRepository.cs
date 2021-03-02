namespace ImagineCommunications.GamePlan.Domain.Shared.System.Products
{
    public interface IProductSettingsRepository
    {
        ProductSettings Get(int id);

        void Add(ProductSettings productSettings);

        void Update(ProductSettings productSettings);
    }
}
