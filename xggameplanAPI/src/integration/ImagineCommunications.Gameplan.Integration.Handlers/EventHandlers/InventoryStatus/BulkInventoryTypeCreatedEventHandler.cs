using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Objects;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Repositories;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.InventoryStatus.InventoryType;
using ImagineCommunications.Gameplan.Integration.Handlers.Common;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.InventoryStatus
{
    public class BulkInventoryTypeCreatedEventHandler : IEventHandler<IBulkInventoryTypeCreated>
    {
        private readonly IMapper _mapper;
        private readonly IInventoryTypeRepository _inventoryTypeRepository;
        private readonly ILockTypeRepository _lockTypeRepository;

        public BulkInventoryTypeCreatedEventHandler(IMapper mapper, IInventoryTypeRepository inventoryTypeRepository, ILockTypeRepository lockTypeRepository)
        {
            _mapper = mapper;
            _inventoryTypeRepository = inventoryTypeRepository;
            _lockTypeRepository = lockTypeRepository;
        }

        public void Handle(IBulkInventoryTypeCreated command)
        {
            ValidateLockTypes(command);

            _inventoryTypeRepository.AddRange(_mapper.Map<List<InventoryType>>(command.Data.ToList()));
            _inventoryTypeRepository.SaveChanges();
        }

        private void ValidateLockTypes(IBulkInventoryTypeCreated command)
        {
            var lockTypes = _lockTypeRepository.GetAll().Select(c => c.LockType).ToList();
            var inputLockTypes = command.Data.Where(c => c.LockTypes != null && c.LockTypes.Any())
                .SelectMany(x => x.LockTypes).Distinct().ToList();
            var notFoundLockTypes = inputLockTypes.Except(lockTypes).ToList();

            if (notFoundLockTypes.Any())
            {
                throw new DataSyncException(DataSyncErrorCode.LockTypeNotFound,
                    $"Lock types are invalid: {string.Join(", ", notFoundLockTypes)}");
            }
        }
    }
}
