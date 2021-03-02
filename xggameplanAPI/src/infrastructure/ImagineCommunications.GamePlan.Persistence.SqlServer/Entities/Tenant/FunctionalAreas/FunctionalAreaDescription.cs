using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.FunctionalAreas
{
    public class FunctionalAreaDescription: IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public string LanguageAbbreviation { get; set; }
        public string Description { get; set; }

        public Guid FunctionalAreaId { get; set; }
    }
}
