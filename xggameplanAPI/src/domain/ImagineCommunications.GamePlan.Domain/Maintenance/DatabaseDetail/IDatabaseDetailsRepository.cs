namespace ImagineCommunications.GamePlan.Domain.Maintenance.DatabaseDetail
{
    public interface IDatabaseDetailsRepository
    {
        DatabaseDetails Find(int id);

        void Add(DatabaseDetails databaseDetails);

        void Update(DatabaseDetails databaseDetails);

        void Remove(int id);
    }
}
