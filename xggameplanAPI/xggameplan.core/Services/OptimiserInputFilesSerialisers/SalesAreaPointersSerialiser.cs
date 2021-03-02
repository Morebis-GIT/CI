using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using xggameplan.Extensions;
using xggameplan.Model.AutoGen;

namespace xggameplan.core.Services.OptimiserInputFilesSerialisers
{
    /// <summary>
    /// A list of all sales areas.
    /// </summary>
    public class SalesAreaPointersSerialiser
    {
        public SalesAreaPointersSerialiser(string folderName)
        {
            if (String.IsNullOrWhiteSpace(folderName))
            {
                throw new ArgumentNullException(nameof(folderName));
            }

            FolderName = folderName;
        }

        private string FolderName { get; }

        /// <summary>
        /// All sales areas filename.
        /// </summary>
        public static string Filename => "v_sare_list.xml";

        public void Serialise(
            IReadOnlyCollection<SalesArea> salesAreas,
            IReadOnlyCollection<Demographic> demographicMetadata,
            IMapper mapper)
        {
            ToAgSalesPtr(salesAreas, demographicMetadata, mapper)
                .Serialize(Path.Combine(FolderName, Filename));
        }

        private static AgSalesAreaPtrsSerialization ToAgSalesPtr(
            IEnumerable<SalesArea> salesAreas,
            IReadOnlyCollection<Demographic> demographicMetadata,
            IMapper mapper)
        {
            var agSalesArea = salesAreas
                .Select(s => mapper.Map<AgSalesAreaPtr>(
                    Tuple.Create(s, demographicMetadata))
                )
                .ToList();

            var salesAreaSerialization = new AgSalesAreaPtrsSerialization();

            return salesAreaSerialization.MapFrom(agSalesArea);
        }
    }
}
