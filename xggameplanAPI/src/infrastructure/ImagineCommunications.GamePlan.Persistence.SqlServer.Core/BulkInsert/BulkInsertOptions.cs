using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert
{
    public class BulkInsertOptions
    {
        /// <summary>
        /// Indicates whether the order of entities is prepared for bulk insert.
        /// Has effect when <see cref="BulkConfig.PreserveInsertOrder"/> is true.
        /// </summary>
        public bool InsertOrderPrepared { get; set; }

        /// <summary>
        /// Indicates whether the guid value is generated for empty <see cref="System.Guid"/> primary key properties
        /// in case they have HasDefaultValueSql("newid()") declaration in their entity configuration;
        /// </summary>
        public bool GenerateValueForEmptyGuidPk { get; set; } = true;
        public bool PreserveInsertOrder { get; set; }
        public int BatchSize { get; set; }
        public int BulkCopyTimeout { get; set; }
        public List<string> PropertiesToInclude { get; set; }
        public bool SetOutputIdentity { get; set; }
    }
}
