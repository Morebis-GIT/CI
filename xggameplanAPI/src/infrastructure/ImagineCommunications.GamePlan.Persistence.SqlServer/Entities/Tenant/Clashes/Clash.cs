using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class Clash : IUniqueIdentifierPrimaryKey
    {
        public const string SearchField = "TokenizedName";
        public static readonly IReadOnlyList<string> SearchFieldSources = new List<string>() { nameof(Externalref), nameof(Description) }.AsReadOnly();

        Guid ISinglePrimaryKey<Guid>.Id
        {
            get => Uid;
            set => Uid = value;
        }

        public Guid Uid { get; set; }
        public string Externalref { get; set; }
        public string ParentExternalidentifier { get; set; }
        public string Description { get; set; }
        public int DefaultPeakExposureCount { get; set; }
        public int DefaultOffPeakExposureCount { get; set; }

        public ICollection<ClashDifference> Differences { get; set; } = new List<ClashDifference>(0);
    }
}
