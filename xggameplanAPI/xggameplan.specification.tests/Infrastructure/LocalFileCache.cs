using System;
using System.IO;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.Infrastructure
{
    public class LocalFileCache : ILocalFileCache
    {
        private readonly string _cacheFolderName = Guid.NewGuid().ToString();

        protected string GetFullFileName(string fileName)
        {
            return Path.Combine(RootFolder, fileName);
        }

        public string RootFolder =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LocalFileCache", _cacheFolderName);

        public void Save(string fileName, Stream stream)
        {
            _ = Directory.CreateDirectory(RootFolder);
            using (var fs = new FileStream(GetFullFileName(fileName), FileMode.Create))
            {
                stream.CopyTo(fs);
                fs.Flush();
            }
        }

        public void Remove(string fileName)
        {
            if (Exists(fileName))
            {
                File.Delete(GetFullFileName(fileName));
            }
        }

        public bool Exists(string fileName) => File.Exists(GetFullFileName(fileName));
    }
}
