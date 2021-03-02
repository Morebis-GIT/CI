using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.Extensions
{
    /// <summary>
    /// Extends <see cref="MigrationBuilder"/> functionality.
    /// </summary>
    public static class MigrationBuilderExtensions
    {
        /// <summary>
        /// Creates a trigger for handling computed fields for MySql 5.6
        /// It might be useful for migration.
        /// </summary>
        /// <param name="migrationBuilder"></param>
        /// <param name="table"> Name of the table for which new trigger will be created</param>
        /// <param name="columnName"> Name of the field to set the value </param>
        /// <param name="expression"> string expression to calculate value </param>

        public static MigrationBuilder CreateTokenizedNameCalculation(this MigrationBuilder migrationBuilder, string table, string expression, string columnName = "TokenizedName")
        {
            migrationBuilder.Sql($"CREATE TRIGGER ComputationValueFor{table + columnName}onInsert " +
               $"BEFORE INSERT ON `{table}` " +
               $"FOR EACH ROW " +
                   $"SET NEW.{columnName} = {expression};");

            migrationBuilder.Sql($"CREATE TRIGGER ComputationValueFor{table + columnName}onUpdate " +
               $"BEFORE UPDATE ON `{table}` " +
               $"FOR EACH ROW " +
                   $"SET NEW.{columnName} = {expression};");
            return migrationBuilder;
        }

        public static MigrationBuilder SetCasesensitiveColumn(this MigrationBuilder migrationBuilder, string columnName, string table,  int maxLenght, bool nullable)
        {
            var nullableExpr = nullable ? "NULL" : "NOT NULL";
            migrationBuilder.Sql($"ALTER TABLE `{table}` " +
                   $"MODIFY COLUMN {columnName} VARCHAR({maxLenght}) COLLATE utf8_bin " +
                   $"{nullableExpr}");
            return migrationBuilder;
        }


        public static MigrationBuilder SetLongTextColumn(this MigrationBuilder migrationBuilder, string columnName, string table, bool nullable)
        {
            var nullableExpr = nullable ? "NULL" : "NOT NULL";
            migrationBuilder.Sql($"ALTER TABLE `{table}`" +
                   $"MODIFY COLUMN {columnName} LONGTEXT COLLATE utf8_bin " +
                   $"{nullableExpr}");
            return migrationBuilder;
        }

    }
}
