using System.Linq;
using AutoMapper;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Product;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Product
{
    public class SqlServerBulkProductCreatedOrUpdatedEventHandler : ImagineCommunications.BusClient.Abstraction.Classes.EventHandler<IBulkProductCreatedOrUpdated>
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public SqlServerBulkProductCreatedOrUpdatedEventHandler(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public override void Handle(IBulkProductCreatedOrUpdated command)
        {
            var items = command?.Data ?? Enumerable.Empty<IProductCreatedOrUpdated>();
            new ProductRuntimeUpdater(_dbContext, _mapper, items).Update();
        }
    }
}
