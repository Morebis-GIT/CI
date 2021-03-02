using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions
{
    public static class EntityTypeBuilderExtensions
    {
        /// <summary>
        /// The annotation key for search fields
        /// </summary>
        public const string SearchFieldAnnotation = "MySql:SearchFieldName";

        /// <summary>
        /// Creates and configures a property for full-text search,which is calcuated from searchFieldSources
        /// </summary>
        /// <param name="entityTypeBuilder"></param>
        /// <param name="searchFieldName">The property name of the calculated field</param>
        /// <param name="searchFieldSources">Fields to compute</param>
        /// <returns>Returns the same instance of an EntityTypeBuilder for futher configurations</returns>
        public static EntityTypeBuilder HasFtsField(this EntityTypeBuilder entityTypeBuilder, string searchFieldName, IReadOnlyList<string> searchFieldSources)
        {
            _ = entityTypeBuilder.Property<string>(searchFieldName)
                    .AsFts(searchFieldSources)
                    .HasMaxLength(TextColumnLenght.SearchField);//configures  a field for full text search

           _ =  entityTypeBuilder.HasAnnotation(SearchFieldAnnotation, searchFieldName);//this is required for further unpacking of this value in migration execution

            return entityTypeBuilder;
        }


    }
}
