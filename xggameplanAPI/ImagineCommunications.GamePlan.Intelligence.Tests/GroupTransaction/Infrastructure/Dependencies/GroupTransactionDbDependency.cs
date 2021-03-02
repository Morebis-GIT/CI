using System;
using AutoMapper;
using BoDi;
using ImagineCommunications.BusClient.Domain.Abstractions.Repositories;
using ImagineCommunications.Gameplan.Integration.Data.Context;
using ImagineCommunications.Gameplan.Integration.Data.Repositories;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.GroupTransaction.Infrastructure.Dependencies
{
    public class GroupTransactionDbDependency : IScenarioDependency
    {
        public void Register(IObjectContainer objectContainer)
        {
            var options = new DbContextOptionsBuilder<IntelligenceDbContext>()
                .UseInMemoryDatabase(databaseName: "IntelligenceDb_" + Guid.NewGuid().ToString())
                .Options;
            var sqlContext = new IntelligenceDbContext(options);
            objectContainer.RegisterInstanceAs<IntelligenceDbContext>(sqlContext);
            objectContainer.RegisterInstanceAs<IMessageInfoRepository>(new MessageInfoRepository(sqlContext, objectContainer.Resolve<IMapper>()));
            objectContainer.RegisterInstanceAs<IMessagePayloadRepository>(new MessagePayloadRepository(sqlContext, objectContainer.Resolve<IMapper>()));
            objectContainer.RegisterInstanceAs<IGroupTransactionInfoRepository>(new GroupTransactionInfoRepository(sqlContext, objectContainer.Resolve<IMapper>()));
            objectContainer.RegisterInstanceAs<IMessagePriorityRepository>(new MessagePriorityRepository(sqlContext, objectContainer.Resolve<IMapper>()));
        }
    }
}
