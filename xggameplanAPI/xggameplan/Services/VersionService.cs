using System.IO;
using System.Reflection;
using xggameplan.Model;

namespace xggameplan.Services
{
    public static class VersionService
    {
        internal static APIVersionModel GetVersion()
        {
            APIVersionModel apiVersionModel = null;

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("xggameplan.version.txt"))
            using (var reader = new StreamReader(stream))
            {
                apiVersionModel = APIVersionModel.Create(reader.ReadToEnd());
            }

            return apiVersionModel;
        }
    }
}
