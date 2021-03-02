namespace ImagineCommunications.GamePlan.Domain.Generic.Interfaces
{
    public interface IImporter<in TSource>
    {
        void Import(TSource source);
    }
}
