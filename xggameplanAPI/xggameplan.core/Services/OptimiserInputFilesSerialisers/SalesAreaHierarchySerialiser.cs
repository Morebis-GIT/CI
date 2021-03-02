using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using xggameplan.Extensions;
using xggameplan.Model.AutoGen;

namespace xggameplan.core.Services.OptimiserInputFilesSerialisers
{
    /// <summary>
    /// National / regional hierarchy of sales areas.
    /// </summary>
    public class SalesAreaHierarchySerialiser
    {
        public SalesAreaHierarchySerialiser(string folderName)
        {
            if (String.IsNullOrWhiteSpace(folderName))
            {
                throw new ArgumentNullException(nameof(folderName));
            }

            FolderName = folderName;
        }

        private string FolderName { get; }

        /// <summary>
        /// Cross-reference list of national / regional sales areas filename.
        /// </summary>
        public static string Filename => "v_xref_list.xml";

        public void Serialise(
            IReadOnlyCollection<SalesArea> salesAreas,
            IMapper mapper)
        {
            ToAgSalesRef(salesAreas, mapper)
                .Serialize(Path.Combine(FolderName, Filename));
        }

        private static AgSalesAreaRefsSerialization ToAgSalesRef(
            IEnumerable<SalesArea> salesAreas,
            IMapper mapper)
        {
            var idList = salesAreas
                .Select(s => s.CustomId)
                .ToList();

            var agSalesAreaRef = mapper.Map<List<AgSalesAreaRef>>(idList);
            var salesAreaSerialization = new AgSalesAreaRefsSerialization();

            return salesAreaSerialization.MapFrom(agSalesAreaRef);
        }
    }
}
