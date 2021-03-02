using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using xggameplan.common.Utilities;

namespace xggameplan.Extensions
{
    public static class XmlConversion
    {
        /// <summary>
        /// Convert into XML. Store XML into a file with the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataToSerialize">Any aribitary object</param>
        /// <param name="filePath">File storage location with file name</param>
        public static void Serialize<T>(this T dataToSerialize, string filePath)
        {
            if (dataToSerialize == null)
            {
                throw new ArgumentNullException(nameof(dataToSerialize));
            }

            if (String.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);

            var serializer = new XmlSerializer(dataToSerialize.GetType());
            var settings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(false)
            };

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            using (var writer = XmlWriter.Create(fileStream, settings))
            using (var customXmlWriter = new CustomXmlWriter(writer) { AlwaysWriteFullEndElement = true })
            {
                customXmlWriter.WriteDocType("boost_serialization", "", "xGG", "");
                serializer.Serialize(customXmlWriter, dataToSerialize, namespaces);
            }
        }

        /// <summary>
        /// Get the List of T and serialize into XML Memory stream
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataToSerialize"></param>
        /// <returns></returns>
        public static MemoryStream Serialize<T>(this T dataToSerialize)
        {
            try
            {
                if (dataToSerialize == null)
                {
                    throw new ArgumentNullException();
                }

                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add("", "");

                var serializer = new XmlSerializer(dataToSerialize.GetType());
                var settings = new XmlWriterSettings
                {
                    Encoding = new UTF8Encoding(false)
                };

                var memstream = new MemoryStream();
                var writer = XmlWriter.Create(memstream, settings);
                using (var customXmlWriter = new CustomXmlWriter(writer) { AlwaysWriteFullEndElement = true })
                {
                    customXmlWriter.WriteDocType("boost_serialization", "", "xGG", "");
                    serializer.Serialize(customXmlWriter, dataToSerialize, namespaces);
                }

                return memstream;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// deserialize file and return as List of data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T Deserialize<T>(this string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    throw new ArgumentNullException();
                }

                var serializer = new XmlSerializer(typeof(T));
                using (var filereader = XmlReader.Create(filePath))
                {
                    return (T)serializer.Deserialize(filereader);
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
