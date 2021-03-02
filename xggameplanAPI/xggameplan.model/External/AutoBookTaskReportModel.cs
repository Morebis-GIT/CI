using System;

namespace xggameplan.model.External
{
    public class AutoBookTaskReportModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Time created.
        /// </summary>
        public DateTime TimeCreated { get; set; }

        /// <summary>
        /// Run Id.
        /// </summary>
        public Guid RunId { get; set; }

        /// <summary>
        /// Scenario Id.
        /// </summary>
        public Guid ScenarioId { get; set; }

        /// <summary>
        /// Url.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Region.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// BinariesVersion.
        /// </summary>
        public string BinariesVersion { get; set; }

        /// <summary>
        /// Version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// FullName.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// InstanceType.
        /// </summary>
        public string InstanceType { get; set; }

        /// <summary>
        /// StorageSizeGB.
        /// </summary>
        public string StorageSizeGB { get; set; }
    }
}
