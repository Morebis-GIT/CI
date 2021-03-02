namespace xggameplan.utils.seeddata.Migration
{
    public interface IMigrationDocumentHandler<TDomainModel> : IMigrationDocumentHandler
        where TDomainModel : class
    {
    }

    public interface IMigrationDocumentHandler
    {
        bool Validate();
        int Execute();
    }
}
