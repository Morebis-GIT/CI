using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.Extensions
{
    /// <summary>
    /// Extends <see cref="MigrationBuilder"/> functionality.
    /// </summary>
    public static class MigrationBuilderExtensions
    {
        /// <summary>
        /// Creates full-text search feilds and triggers for all entites which have Fts field configured in entity configurations
        /// </summary>
        /// <param name="migrationBuilder"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static MigrationBuilder CreateFtsFields(this MigrationBuilder migrationBuilder, IModel model)
        {
            var matchingTypes = model.GetEntityTypes().Where(e => e.FindAnnotation(EntityTypeBuilderExtensions.SearchFieldAnnotation) != null);
            foreach (var et in matchingTypes)
            {
                var fieldName = et.FindAnnotation(EntityTypeBuilderExtensions.SearchFieldAnnotation).Value as string;
                var _fieldsToInclude = (et.FindProperty(fieldName).FindAnnotation(PropertyBuilderExtensions.SearchFieldAnnotationName).Value as string).Split(",");
                _ = migrationBuilder.ConfigureFullTextWithTriggers(et, _fieldsToInclude, fieldName);
            }
            return migrationBuilder;
        }

        /// <summary>
        /// Create a fulltext index and triggers for a specific entity type,which is not configured for fulltext search in Entity Configurations
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="migrationBuilder"></param>
        /// <param name="model"></param>
        /// <param name="fieldsToInclude"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static MigrationBuilder CreateFtsField<T>(this MigrationBuilder migrationBuilder, IModel model, IReadOnlyList<string> fieldsToInclude, string fieldName = "TokenizedSearch") where T : class
        {
            var et = model.FindEntityType(typeof(T).FullName);
            _ = migrationBuilder.ConfigureFullTextWithTriggers(et, fieldsToInclude, fieldName);
            return migrationBuilder;
        }

        /// <summary>
        /// Configures only the FULLTEXT index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="model"></param>
        /// <param name="columnname"></param>
        /// <returns></returns>
        public static MigrationBuilder AddFulltextIndex<T>(this MigrationBuilder builder,IModel model,string columnname = "TokenizedName")
        {
            var et = model.FindEntityType(typeof(T).FullName);
            _ = builder.AddFulltext(et.Relational().TableName, columnname);
            return builder;
        }

        private static MigrationBuilder AddFulltext(this MigrationBuilder builder,string table,string columnName)
        {
            _ = builder.Sql($"ALTER TABLE {table} ADD FULLTEXT({columnName});");
            return builder;
        }

        /// <summary>
        /// Creates a trigger for handling computed fields for MySql 5.6
        /// It might be useful for migration.
        /// </summary>
        /// <param name="migrationBuilder"></param>
        /// <param name="table"> Name of the table for which new trigger will be created</param>
        /// <param name="columnName"> Name of the field to set the value </param>
        /// <param name="expression"> string expression to calculate value </param>
        private static MigrationBuilder CreateTokenizedNameCalculationTriggers(this MigrationBuilder migrationBuilder, string table, string expression, string columnName = "TokenizedName")
        {
            _ = migrationBuilder.Sql($"CREATE TRIGGER Compute_{table}_{columnName}_onInsert " +
          $"BEFORE INSERT ON `{table}` " +
          $"FOR EACH ROW " +
              $"SET NEW.{columnName} = {expression};");

            _ = migrationBuilder.Sql($"CREATE TRIGGER Compute_{table}_{columnName}_onUpdate " +
               $"BEFORE UPDATE ON `{table}` " +
               $"FOR EACH ROW " +
                   $"SET NEW.{columnName} = {expression};");
            return migrationBuilder;
        }

        /// <summary>
        /// Common functionality of CreateFtsField<T> and CreateFtsField
        /// </summary>
        /// <param name="migrationBuilder"></param>
        /// <param name="et"></param>
        /// <param name="_fieldsToInclude"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private static MigrationBuilder ConfigureFullTextWithTriggers(this MigrationBuilder migrationBuilder, IEntityType et, IEnumerable<string> _fieldsToInclude, string fieldName)
        {
            var sb = new StringBuilder($"CONCAT_WS(NEW.{_fieldsToInclude.First()}");
            foreach (var fieldToInclude in _fieldsToInclude.Skip(1))
            {
                _ = sb.Append(",' ',NEW.").Append(fieldToInclude);
            }
            _ = sb.Append(")");
            //here make some if logics
            var table = et.Relational().TableName;
            var expression = sb.ToString();

            _ = migrationBuilder.AddFulltext(table, fieldName);
            _ = migrationBuilder.CreateTokenizedNameCalculationTriggers(table, expression, fieldName);

            return migrationBuilder;
        }

        public static MigrationBuilder SetCasesensitiveColumn(this MigrationBuilder migrationBuilder, string columnName, string table, int maxLenght, bool nullable)
        {
            var nullableExpr = nullable ? "NULL" : "NOT NULL";
            _ = migrationBuilder.Sql($"ALTER TABLE `{table}` " +
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
