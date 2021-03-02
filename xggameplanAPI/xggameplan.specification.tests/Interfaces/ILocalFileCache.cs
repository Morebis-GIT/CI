using System.IO;

namespace xggameplan.specification.tests.Interfaces
{
    public interface ILocalFileCache
    {
        string RootFolder { get; }

        void Save(string fileName, Stream stream);

        void Remove(string fileName);

        bool Exists(string fileName);
    }
}
