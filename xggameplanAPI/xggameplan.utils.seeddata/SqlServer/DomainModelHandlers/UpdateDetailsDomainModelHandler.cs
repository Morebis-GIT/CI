using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Maintenance.UpdateDetail;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using MasterEntitites = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class UpdateDetailsDomainModelHandler : IDomainModelHandler<UpdateDetails>
    {
        private readonly IUpdateDetailsRepository _updateDetailsRepository;
        private readonly ISqlServerDbContext _dbContext;

        public UpdateDetailsDomainModelHandler(IUpdateDetailsRepository updateDetailsRepository, ISqlServerDbContext dbContext)
        {
            _updateDetailsRepository = updateDetailsRepository ?? throw new ArgumentNullException(nameof(updateDetailsRepository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public UpdateDetails Add(UpdateDetails model)
        {
            _updateDetailsRepository.Add(model);
            return model;
        }

        public void AddRange(params UpdateDetails[] models)
        {
            foreach (var model in models)
            {
                _ = Add(model);
            }
        }


        public int Count() => _dbContext.Query<MasterEntitites.UpdateDetails>().Count();

        public void DeleteAll() => _dbContext.Truncate<MasterEntitites.UpdateDetails>();

        public IEnumerable<UpdateDetails> GetAll() => _updateDetailsRepository.GetAll();
    }
}
