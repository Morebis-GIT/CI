using EFCore.BulkExtensions;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert
{
    public class BulkInsertOptions : BulkConfig
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
    }
}
