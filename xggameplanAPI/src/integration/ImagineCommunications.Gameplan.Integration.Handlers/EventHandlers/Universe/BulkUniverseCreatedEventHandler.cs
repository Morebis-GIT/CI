using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Universe;
using ImagineCommunications.Gameplan.Integration.Handlers.Common;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using UniverseDbObject = ImagineCommunications.GamePlan.Domain.Shared.Universes.Universe;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Universe
{
    public class BulkUniverseCreatedEventHandler : BusClient.Abstraction.Classes.EventHandler<IBulkUniverseCreated>
    {
        private readonly IMapper _mapper;
        private readonly IUniverseRepository _universeRepository;
        private readonly ISalesAreaRepository _salesAreaRepository;
        private readonly IDemographicRepository _demographicRepository;

        public BulkUniverseCreatedEventHandler(IMapper mapper,
            IUniverseRepository universeRepository,
            ISalesAreaRepository salesAreaRepository,
            IDemographicRepository demographicRepository)
        {
            _mapper = mapper;
            _universeRepository = universeRepository;
            _salesAreaRepository = salesAreaRepository;
            _demographicRepository = demographicRepository;
        }

        public override void Handle(IBulkUniverseCreated command)
        {
            _salesAreaRepository.ValidateSalesArea(command.Data.Select(x => x.SalesArea).ToList());
            _demographicRepository.ValidateDemographics(command.Data.Select(x => x.Demographic).ToList());

            var universes = _universeRepository.Search(command.Data.Select(x => x.Demographic).Distinct().ToList(),
                command.Data.Select(x => x.SalesArea).Distinct().ToList(),
                DateTime.MinValue, DateTime.MinValue).ToList();

            SaveUniverse(command.Data.ToList(), universes);
            _universeRepository.SaveChanges();
        }

        private void SaveUniverse(List<IUniverseCreated> newUniverses, List<UniverseDbObject> existingUniverses)
        {
            var universes = new List<UniverseDbObject>();
            newUniverses.OrderBy(d => d.StartDate).GroupBy(g => new { Demographic = g.Demographic.ToUpperInvariant(), Salesarea = g.SalesArea }).ToList().ForEach(
                g =>
                {
                    var existingUniverse = existingUniverses?.Where(
                         _ => _.Demographic.Equals(g.Key.Demographic, StringComparison.OrdinalIgnoreCase) &&
                              _.SalesArea.Equals(g.Key.Salesarea)).OrderByDescending(_ => _.EndDate).FirstOrDefault();

                    g.ToList().ForEach(item =>
                    {
                        SaveUniverse(item, existingUniverse, out UniverseDbObject nextUniverse, ref universes);
                        existingUniverse = nextUniverse;
                    });
                });
            if (universes.Any())
            {
                _universeRepository.Insert(universes);
            }
        }

        private void SaveUniverse(IUniverseCreated command, UniverseDbObject lastUniverse, out UniverseDbObject nextUniverse, ref List<UniverseDbObject> universes)
        {
            nextUniverse = null;
            if (lastUniverse == null)
            {
                nextUniverse = MapEntity(command);
                AddOrUpdate(ref universes, nextUniverse); // "Add"
                return;
            }

            if (command.StartDate < lastUniverse.StartDate.Date)
            {
                throw new DataSyncException(DataSyncErrorCode.Universe_StartDateLessThanPredecessors,
                    "universe start date(" + command.StartDate + ") should be greater than or equal to existing universe start date(" +
                    lastUniverse.StartDate + "). Sales area:" + command.SalesArea + ", Demographic:" + command.Demographic);
            }

            if (command.StartDate == lastUniverse.StartDate.Date) //"update/overwrite";
            {
                lastUniverse.EndDate = command.EndDate;
                lastUniverse.UniverseValue = command.UniverseValue;
                nextUniverse = lastUniverse;
                AddOrUpdate(ref universes, lastUniverse);
                return;
            }

            if (command.StartDate > lastUniverse.StartDate && command.EndDate <= lastUniverse.EndDate)
            {
                throw new DataSyncException(DataSyncErrorCode.Universe_DateRangeOverlapsPredecessors,
                    "universe start and end date range should not fully overlap with existing universe start date(" +
                    lastUniverse.StartDate + ") end date (" + lastUniverse.EndDate + "). Sales area:" + command.SalesArea + ", Demographic:" + command.Demographic);
            }

            if (command.StartDate == lastUniverse.EndDate.AddDays(1).Date)
            {
                nextUniverse = MapEntity(command);
                AddOrUpdate(ref universes, nextUniverse); // "Add"
                return;
            }

            if (command.StartDate > lastUniverse.EndDate.AddDays(1).Date)
            {
                throw new DataSyncException(DataSyncErrorCode.Universe_GapMoreThan1Day,
                    "Universe start date should not be greater than(" + lastUniverse.EndDate.AddDays(1).Date +
                    "). Sales area:" + command.SalesArea + ", Demographic:" + command.Demographic);
            }

            if (command.StartDate > lastUniverse.StartDate.Date && command.StartDate <= lastUniverse.EndDate.Date &&
                command.EndDate > lastUniverse.EndDate.Date
            ) //   "Add it as new and also update the existing start date";
            {
                nextUniverse = MapEntity(command);
                AddOrUpdate(ref universes, nextUniverse);

                lastUniverse.EndDate = command.StartDate.AddDays(-1).Date;
                AddOrUpdate(ref universes, lastUniverse);
            }
        }

        private UniverseDbObject MapEntity(IUniverseCreated model)
        {
            var universe = _mapper.Map<UniverseDbObject>(model);
            universe.Id = Guid.NewGuid();

            return universe;
        }

        private static void AddOrUpdate(ref List<UniverseDbObject> universes, UniverseDbObject universe)
        {
            _ = universes.RemoveAll(_ => _.Id.Equals(universe.Id));
            universes.Add(universe);
        }
    }
}
