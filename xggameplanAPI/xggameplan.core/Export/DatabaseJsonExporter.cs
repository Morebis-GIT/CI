using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ImagineCommunications.GamePlan.Domain.Generic.DbContext;
using Newtonsoft.Json;
using xggameplan.Database;

namespace xggameplan.core.Database
{
    /// <summary>
    /// Exports database to files, currently just documents are exported
    /// </summary>
    public class DatabaseJsonExporter : IDatabaseExporter
    {
        private static readonly MethodInfo GetExportMethodInfo = typeof(DatabaseJsonExporter)
           .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
           .FirstOrDefault(m => m.Name == nameof(ExportByDocumentType) && m.IsGenericMethod)?.GetGenericMethodDefinition();

        private readonly JsonSerializer _serializer;
        private readonly IDbContext _dbContext;

        public DatabaseJsonExporter(IDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

            var settings = new JsonSerializerSettings();
            _serializer = JsonSerializer.CreateDefault(settings);
        }

        public void Export(DatabaseMassProcessingSettings databaseExportSettings)
        {
            _ = databaseExportSettings ?? throw new ArgumentNullException(nameof(databaseExportSettings));
            var dataFolder = databaseExportSettings.DataFolder;
            Directory.CreateDirectory(dataFolder);

            databaseExportSettings.GetDocumentTypesToProcess()
                .ForEach(documentType =>
                    GetExportMethodInfo.MakeGenericMethod(documentType).Invoke(this,
                            new object[] { dataFolder,
                                           databaseExportSettings.GetDocumentTypeFilterFunction(documentType),
                                           databaseExportSettings.GetTransformDataDocumentFunction(documentType)}));
        }

        private void ExportByDocumentType<T>(string dataFolder, Func<object, bool> filterDocumentFunction,
                              Action<object> transformDocumentFunction) where T : class
        {
            var query = _dbContext.Query<T>();
            if (filterDocumentFunction != null)
            {
                query = query.Where(i => filterDocumentFunction(i));
            }

            var objects = query.ToList();

            if (transformDocumentFunction != null)
            {
                objects.ForEach(item => transformDocumentFunction(item));
            }

            string outputFileName = Path.Combine(dataFolder, $"{typeof(T).Name}.json");

            if (File.Exists(outputFileName))
            {
                File.Delete(outputFileName);
            }

            using var writer = File.CreateText(outputFileName);
            _serializer.Serialize(writer, objects);
        }
    }
}