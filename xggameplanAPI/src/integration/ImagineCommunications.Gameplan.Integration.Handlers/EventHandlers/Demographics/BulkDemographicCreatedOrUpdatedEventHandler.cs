using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Demographics
{
    public class BulkDemographicCreatedOrUpdatedEventHandler : BusClient.Abstraction.Classes.EventHandler<IBulkDemographicCreatedOrUpdated>
    {
        private readonly IDemographicRepository _demographicsRepository;
        private readonly IMapper _mapper;

        public BulkDemographicCreatedOrUpdatedEventHandler(IDemographicRepository demographicsRepository, IMapper mapper)
        {
            _mapper = mapper;
            _demographicsRepository = demographicsRepository;
        }

        /// <summary>
        /// Handles the command recieved with Masstransit 
        /// </summary>
        /// <param name="command">this is bulk model of demographics that need to be updated in database</param>
        public override void Handle(IBulkDemographicCreatedOrUpdated command)
        {
            var demographicEntities = _mapper.Map<List<Demographic>>(command.Data);
            var existedDemographics = _demographicsRepository.GetByExternalRef(command.Data.Select(c => c.ExternalRef).ToList());
            var demographics = new List<Demographic>();

            foreach (var demographic in demographicEntities)
            {
                var demographicToUpdate = existedDemographics.FirstOrDefault(c => c.ExternalRef == demographic.ExternalRef);
                if (demographicToUpdate == null)
                {
                    demographics.Add(demographic);
                }
                else
                {
                    demographicToUpdate.Update(demographic.Name, demographic.ShortName, demographic.DisplayOrder, demographic.Gameplan);
                    demographics.Add(demographicToUpdate);
                }
            }

            _demographicsRepository.InsertOrUpdate(demographics);
            _demographicsRepository.SaveChanges();
        }
    }
}
