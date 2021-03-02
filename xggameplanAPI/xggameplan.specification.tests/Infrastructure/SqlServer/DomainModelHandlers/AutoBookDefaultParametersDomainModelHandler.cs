using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters;
using ImagineCommunications.GamePlan.Domain.Generic.DbContext;
using Microsoft.EntityFrameworkCore;
using xggameplan.specification.tests.Interfaces;
using AutoBookDefaultParametersEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters.AutoBookDefaultParameters;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.DomainModelHandlers
{
    public class AutoBookDefaultParametersDomainModelHandler : SimpleDomainModelMappingHandler<AutoBookDefaultParametersEntity, AutoBookDefaultParameters>
    {
        private readonly ITenantDbContext _dbContext;

        public AutoBookDefaultParametersDomainModelHandler(ISqlServerTestDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
        }

        public override AutoBookDefaultParameters Add(AutoBookDefaultParameters model)
        {
            var entity = Mapper.Map<AutoBookDefaultParametersEntity>(model);

            _ = _dbContext.Add(entity);

            return model;
        }

        public override void AddRange(params AutoBookDefaultParameters[] models)
        {
            foreach (var model in models)
            {
                _ = Add(model);
            }
        }

        public override IEnumerable<AutoBookDefaultParameters> GetAll()
        {
            var entities = _dbContext.Query<AutoBookDefaultParametersEntity>()
                   .Include(e => e.AgBreak_AgAvals)
                   .Include(e => e.AgBreak_AgPredictions)
                   .Include(e => e.AgBreak_AgRegionalBreaks)
                   .Include(e => e.AgCampaign_AgCampaignSalesAreas).ThenInclude(e => e.AgDayParts).ThenInclude(e => e.AgDayPartLengths)
                   .Include(e => e.AgCampaign_AgCampaignSalesAreas).ThenInclude(e => e.AgDayParts).ThenInclude(e => e.AgTimeSlices)
                   .Include(e => e.AgCampaign_AgCampaignSalesAreas).ThenInclude(e => e.AgLengths).ThenInclude(e => e.AgMultiParts)
                   .Include(e => e.AgCampaign_AgCampaignSalesAreas).ThenInclude(e => e.AgParts)
                   .Include(e => e.AgCampaign_AgCampaignSalesAreas).ThenInclude(e => e.AgPartsLengths)
                   .Include(e => e.AgCampaign_AgCampaignSalesAreas).ThenInclude(e => e.AgStrikeWeights).ThenInclude(e => e.AgStrikeWeightLengths)
                   .Include(e => e.AgCampaign_AgProgrammeList).ThenInclude(e => e.CategoryOrProgramme)
                   .Include(e => e.AgCampaign_AgProgrammeList).ThenInclude(e => e.TimeBands)
                   .Include(e => e.AgHfssDemos)
                   .ToList();

            return Mapper.Map<List<AutoBookDefaultParameters>>(entities);
        }
    }
}
