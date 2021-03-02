using System;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.DbContext;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using xggameplan.utils.seeddata.Infrastructure;

namespace xggameplan.utils.seeddata.Seeding
{
    public class JsonFileImporter<T> : IJsonFileImporter<T>
        where T : class
    {
        public string TableName { get; set; }

        private readonly IDomainModelContext _domainModelContext;
        private readonly IDbContext _dbContext;

        protected virtual void SaveImportedData()
        {
            _dbContext.SaveChanges();
        }

        public JsonFileImporter(IDomainModelContext domainModelContext, IDbContext dbContext)
        {
            _domainModelContext = domainModelContext ?? throw new ArgumentNullException(nameof(domainModelContext));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public bool Import(string dataFolder, bool replaceExistingData)
        {
            var searchPattern = string.IsNullOrEmpty(TableName) ? $"{typeof(T).Name}*.json" : $"{TableName}.json";
            if (Directory.Exists(dataFolder))
            {
                var file = Directory.GetFiles(dataFolder, searchPattern)
                    .FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(file))
                {
                    if (replaceExistingData)
                    {
                        Delete();
                    }
                    var contentString = File.ReadAllText(file);
                    new JsonDocumentImporter<T>(_domainModelContext).Import(contentString);
                    SaveImportedData();
                    return true;
                }
            }

            return false;
        }

        public int GetDocumentCount()
        {
            return _domainModelContext.Count<T>();
        }

        public void Delete()
        {
            _domainModelContext.DeleteAll<T>();
        }
    }
}
