using System;
using System.Collections.Generic;
using System.IO;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using xggameplan.Extensions;
using xggameplan.Model.AutoGen;

namespace xggameplan.core.Services.OptimiserInputFilesSerialisers
{
    public class ClashExposureDetailsSerialiser
    {
        public ClashExposureDetailsSerialiser(string folderName)
        {
            if (String.IsNullOrWhiteSpace(folderName))
            {
                throw new ArgumentNullException(nameof(folderName));
            }

            FolderName = folderName;
        }

        private string FolderName { get; }

        /// <summary>
        /// Clash exposure details filename.
        /// </summary>
        public static string Filename => "v_exposure_dtls.xml";

        public void Serialise(
            List<Clash> allClashes,
            DateTime startDate,
            DateTime endDate,
            TimeSpan? peakStartTime,
            TimeSpan? peakEndTime,
            List<SalesArea> allSalesAreas,
            IMapper mapper,
            AgExposure agExposure)
        {
            ToAgExposures(
                allClashes,
                startDate,
                endDate,
                peakStartTime,
                peakEndTime,
                allSalesAreas,
                mapper,
                agExposure)
            .Serialize(Path.Combine(FolderName, Filename));
        }

        private static AgExposuresSerialization ToAgExposures(
            List<Clash> clashes,
            DateTime runStartDateTime,
            DateTime runEndDateTime,
            TimeSpan? peakStartTime,
            TimeSpan? peakEndTime,
            List<SalesArea> salesAreas,
            IMapper mapper,
            AgExposure agExposure)
        {
            var agExposures = mapper.Map<List<AgExposure>>(
                Tuple.Create(clashes, runStartDateTime, runEndDateTime, peakStartTime, peakEndTime, salesAreas, agExposure)
                );

            var serialization = new AgExposuresSerialization();
            return serialization.MapFrom(agExposures);
        }
    }
}
