using System;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using AutoBookDefaultParametersEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters.AutoBookDefaultParameters;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class AutoBookDefaultParametersRepository : IAutoBookDefaultParametersRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private IAutoBookDefaultParameters _cachedModel;
        private readonly IMapper _mapper;

        public AutoBookDefaultParametersRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public IAutoBookDefaultParameters Get()
        {
            if (_cachedModel == null)
            {
                var entity = _dbContext.Query<AutoBookDefaultParametersEntity>()
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
                   .FirstOrDefault();

                _cachedModel = _mapper.Map<AutoBookDefaultParameters>(entity);
            }

            return _cachedModel;
        }

        public void AddOrUpdate(IAutoBookDefaultParameters autoBookDefaultParameters)
        {
            var entity = _dbContext.Query<AutoBookDefaultParametersEntity>()
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
                .FirstOrDefault();

            if (entity == null)
            {
                entity = _mapper.Map<AutoBookDefaultParametersEntity>(autoBookDefaultParameters);

                _dbContext.Add(entity, post => post.MapTo(autoBookDefaultParameters), _mapper);
            }
            else
            {
                _mapper.Map(autoBookDefaultParameters, entity);

                _dbContext.Update(entity, post => post.MapTo(autoBookDefaultParameters), _mapper);
            }
            
            _cachedModel = autoBookDefaultParameters;
        }

        public void SaveChanges() => _dbContext.SaveChanges();
    }    
}
