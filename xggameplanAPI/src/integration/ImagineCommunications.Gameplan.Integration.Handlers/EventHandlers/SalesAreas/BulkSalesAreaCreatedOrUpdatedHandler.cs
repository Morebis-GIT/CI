using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreaDemographics;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.SalesArea;
using ImagineCommunications.Gameplan.Integration.Handlers.Common;
using ImagineCommunications.BusClient.Abstraction.Classes;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.SalesAreas
{
    public class BulkSalesAreaCreatedOrUpdatedHandler : EventHandler<IBulkSalesAreaCreatedOrUpdated>
    {
        private readonly IMapper _mapper;
        private readonly ISalesAreaRepository _salesAreaRepository;
        private readonly IDemographicRepository _demographicRepository;
        private readonly ISalesAreaDemographicRepository _salesAreaDemographicRepository;

        public BulkSalesAreaCreatedOrUpdatedHandler(
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

        public override void Handle(IBulkSalesAreaCreatedOrUpdated command)
        {
            ValidateSalesAreaDemographics(command);

            var customIds = command.Data.Select(x => x.CustomId).ToList();
            var existingSalesAreas = _salesAreaRepository.FindByIds(customIds)
                .ToDictionary(x => x.CustomId);

            var salesAreasWithDemographics = new Dictionary<SalesArea, List<SalesAreaDemographic>>();
            var resultSalesAreas = new List<SalesArea>();

            foreach (var item in command.Data)
            {
                var demographics = _mapper.Map<List<SalesAreaDemographic>>(item.Demographics);
                var newSalesArea = _mapper.Map<SalesArea>(item);

                if (existingSalesAreas.TryGetValue(item.CustomId, out SalesArea salesArea))
                {
                    UpdateSalesAreaModel(salesArea, newSalesArea);
                }
                else
                {
                    salesArea = newSalesArea;
                    salesArea.Id = System.Guid.NewGuid();
                }

                resultSalesAreas.Add(salesArea);
                salesAreasWithDemographics.Add(salesArea, demographics);
            }

            _salesAreaRepository.Update(resultSalesAreas);
            UpdateSalesAreaDemographics(salesAreasWithDemographics);
            _salesAreaDemographicRepository.SaveChanges();
        }

        private void UpdateSalesAreaDemographics(Dictionary<SalesArea, List<SalesAreaDemographic>> salesAreasWithDemographics)
        {
            foreach (var salesArea in salesAreasWithDemographics)
            {
                var demographics = salesArea.Value;
                demographics.ForEach(x => x.SalesArea = salesArea.Key.Name);
            }
            var salesAreaNames = salesAreasWithDemographics.Select(x => x.Key.Name).ToList();
            var allDemographics = salesAreasWithDemographics.SelectMany(x => x.Value).ToList();

            _salesAreaDemographicRepository.DeleteBySalesAreaNames(salesAreaNames);
            _salesAreaDemographicRepository.AddRange(allDemographics);
        }

        private void ValidateSalesAreaDemographics(IBulkSalesAreaCreatedOrUpdated command)
        {
            var baseDemographicExternalRefs = command.Data
                .Select(x => new[] { x.BaseDemographic1, x.BaseDemographic2 })
                .SelectMany(x => x).Distinct().ToList();

            _demographicRepository.ValidateDemographics(baseDemographicExternalRefs);

            foreach (var salesArea in command.Data)
            {
                var commandDemographics = salesArea.Demographics;
                var externalRefs = commandDemographics.Select(x => x.ExternalRef).Distinct().ToArray();
                if (externalRefs.Length != commandDemographics.Count)
                {
                    throw new DataSyncException(DataSyncErrorCode.DuplicateExternalReference, "Duplicated demographic externalRefs");
                }
                _demographicRepository.ValidateDemographics(externalRefs);
            }
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
            salesArea.Name = newSalesArea.Name;
            salesArea.ShortName = newSalesArea.ShortName;
        }
    }
}
