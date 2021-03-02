using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Objects;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Repositories;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.InventoryStatus.LockType;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.InventoryStatus
{
    public class BulkLockTypeCreatedEventHandler : IEventHandler<IBulkLockTypeCreated>
    {
        private readonly IMapper _mapper;
        private readonly ILockTypeRepository _lockTypeRepository;

        public BulkLockTypeCreatedEventHandler(IMapper mapper, ILockTypeRepository lockTypeRepository)
        {
            _mapper = mapper;
            _lockTypeRepository = lockTypeRepository;
        }

        public void Handle(IBulkLockTypeCreated command)
        {
            var lockTypes = _mapper.Map<List<InventoryLockType>>(command.Data.ToList());

            _lockTypeRepository.AddRange(lockTypes);
            _lockTypeRepository.SaveChanges();
        }
    }
}
