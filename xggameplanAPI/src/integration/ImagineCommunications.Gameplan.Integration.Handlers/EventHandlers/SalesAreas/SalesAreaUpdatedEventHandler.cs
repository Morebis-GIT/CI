using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.Gameplan.Integration.Handlers.Common;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.SalesArea;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreaDemographics;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.SalesAreas
{
    public class SalesAreaUpdatedEventHandler : BusClient.Abstraction.Classes.EventHandler<ISalesAreaUpdated>
    {
        private readonly IMapper _mapper;
        private readonly ISalesAreaRepository _salesAreaRepository;
        private readonly IDemographicRepository _demographicRepository;
        private readonly ISalesAreaDemographicRepository _salesAreaDemographicRepository;

        public SalesAreaUpdatedEventHandler(
            ISalesAreaRepository salesAreaRepository,
            IDemographicRepository demographicRepository,
            ISalesAreaDemographicRepository salesAreaDemographicRepository,
            IMapper mapper)
        {
            _salesAreaRepository = salesAreaRepository;
            _demographicRepository = demographicRepository;
            _salesAreaDemographicRepository = salesAreaDemographicRepository;
            _mapper = mapper;
        }

        public override void Handle(ISalesAreaUpdated command)
        {
            var salesArea = _salesAreaRepository.FindByShortName(command.ShortName);
            if (salesArea == null)
            {
                throw new DataSyncException(DataSyncErrorCode.SalesAreaNotFound, "no sales area found");
            }

            _demographicRepository.ValidateDemographics(new List<string> { command.BaseDemographic1, command.BaseDemographic2 });
            ValidateSalesAreaDemographics(command);

            var newSalesArea = _mapper.Map<SalesArea>(command);
            UpdateSalesAreaModel(salesArea, newSalesArea);
            _salesAreaRepository.Update(salesArea);

            var demographics = _mapper.Map<IEnumerable<SalesAreaDemographic>>(command.Demographics).ToList();
            demographics.ForEach(x => x.SalesArea = salesArea.Name);
            UpdateSalesAreaDemographics(salesArea, demographics);

            _salesAreaRepository.SaveChanges();
        }

        private void UpdateSalesAreaModel(SalesArea salesArea, SalesArea newSalesArea)
        {
            salesArea.BaseDemographic1 = newSalesArea.BaseDemographic1;
            salesArea.BaseDemographic2 = newSalesArea.BaseDemographic2;
            salesArea.TargetAreaName = newSalesArea.TargetAreaName;
            salesArea.CurrencyCode = newSalesArea.CurrencyCode;
            salesArea.ChannelGroup = newSalesArea.ChannelGroup;
            salesArea.StartOffset = newSalesArea.StartOffset;
            salesArea.DayDuration = newSalesArea.DayDuration;
            salesArea.CustomId = newSalesArea.CustomId;
        }

        private void UpdateSalesAreaDemographics(SalesArea salesArea, List<SalesAreaDemographic> demographics)
        {
            _salesAreaDemographicRepository.DeleteBySalesAreaName(salesArea.Name);
            _salesAreaDemographicRepository.AddRange(demographics);
        }

        private void ValidateSalesAreaDemographics(ISalesAreaUpdated command)
        {
            var commandDemographics = command.Demographics;
            var externalRefs = commandDemographics.Select(x => x.ExternalRef).Distinct().ToArray();
            if (externalRefs.Length != commandDemographics.Count)
            {
                throw new DataSyncException(DataSyncErrorCode.DuplicateExternalReference, "Duplicated demographic externalRefs");
            }
            _demographicRepository.ValidateDemographics(externalRefs);
        }
    }
}
