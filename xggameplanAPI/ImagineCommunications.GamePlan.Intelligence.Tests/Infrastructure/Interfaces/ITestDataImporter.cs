using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using TechTalk.SpecFlow;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces
{
    public interface ITestDataImporter : IImporter<Stream>
    {
        IEnumerable<TEntity> GetDataFromFile<TEntity>(string fileName, string key) where TEntity : class;
        void ImportDataFromTable(string key, Table table);
    }
}
