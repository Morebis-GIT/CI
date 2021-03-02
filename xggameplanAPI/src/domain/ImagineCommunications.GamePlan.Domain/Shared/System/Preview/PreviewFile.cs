using System;
using System.IO;

namespace ImagineCommunications.GamePlan.Domain.Shared.System.Preview
{
    public class PreviewFile
    {
        private PreviewFile() { }

        public string Id { get; private set; }

        public string FileName { get; private set; }

        public string FileExtension { get; private set; }
        public string ContentType { get; private set; }
        public long? ContentLength { get; private set; }

        private byte[] _content;

        public void SetContent(byte[] content) => _content = content;
        public byte[] GetContent() => _content;


        public static PreviewFile Create(string filename, string contentType,
                                         long? contentLength, string location)
        {
            var fName = $"{Guid.NewGuid().ToString()}-{Path.GetFileName(filename.Replace("\"",""))}";
            return new PreviewFile()
            {
                Id = $"{location}/{fName}", 
                FileName = fName,
                ContentType = contentType,
                ContentLength = contentLength,
                FileExtension = Path.GetExtension(fName)
            };
        }
    }
}
