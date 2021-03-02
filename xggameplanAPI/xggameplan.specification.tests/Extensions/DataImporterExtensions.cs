using System;
using System.Reflection;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.Extensions
{
    public static class DataImporterExtensions
    {
        public static void FromResourceScript(this ITestDataImporter dataImporter, string resourceName)
        {
            if (dataImporter == null)
            {
                throw new ArgumentNullException(nameof(dataImporter));
            }

            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(assembly.GetFullManifestResourceName(resourceName)))
            {
                dataImporter.Import(stream);
            }
        }
    }
}
