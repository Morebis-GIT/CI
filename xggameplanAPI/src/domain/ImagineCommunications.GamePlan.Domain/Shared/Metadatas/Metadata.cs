using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Shared.Metadatas
{
    /// <summary>
    /// class to store seed data in DB
    /// </summary>
    public class Metadata
    {
        /// <summary>
        /// Meta data value with key value
        /// </summary>
        public IDictionary<MetaDataKeys, string> Dictionary { get; set; }
    }

    /// <summary>
    /// Meta data each value entry
    /// </summary>
    public class Data
    {
        /// <summary>
        /// unique id
        /// </summary>
        public int Id { set; get; }
        /// <summary>
        /// meta data value
        /// </summary>
        public object Value { set; get; }

        /// <summary>
        /// Indexes list by Id
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public static Dictionary<int, Data> IndexListById(IEnumerable<Data> datas)
        {
            var dataIndexed = new Dictionary<int, Data>();
            foreach (var data in datas)
            {
                if (!dataIndexed.ContainsKey(data.Id))
                {
                    dataIndexed.Add(data.Id, data);
                }
            }
            return dataIndexed;
        }
    }

    /// <summary>
    /// Domain model for metadata
    /// </summary>
    public class MetadataModel : Dictionary<MetaDataKeys, List<Data>>
    {
        public MetadataModel()
        {
        }

        public MetadataModel(Dictionary<MetaDataKeys, List<Data>> dictionary) : base(dictionary)
        {
        }
    }
}
