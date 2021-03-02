using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Shared.System.Products;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using MasterEntities = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class ProductSettingsDomainModelHandler : IDomainModelHandler<ProductSettings>
    {
        private readonly IProductSettingsRepository _productSettingsRepository;
        private readonly ISqlServerMasterDbContext _dbContext;
        private readonly IMapper _mapper;

        public ProductSettingsDomainModelHandler(
            IProductSettingsRepository previewRefsRepository,
            ISqlServerMasterDbContext dbContext,
            IMapper mapper)
        {
            _productSettingsRepository = previewRefsRepository;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public ProductSettings Add(ProductSettings model)
        {
            _productSettingsRepository.Add(model);
            return model;
        }

        public void AddRange(params ProductSettings[] models)
        {
            foreach (var model in models)
            {
                _ = Add(model);
            }
        }

        public int Count() =>
            _dbContext.Query<MasterEntities.ProductSettings>().Count();

        public void DeleteAll() =>
            _dbContext.Truncate<MasterEntities.ProductSettings>();

        public IEnumerable<ProductSettings> GetAll() =>
            _dbContext.Query<MasterEntities.ProductSettings>()
            .ProjectTo<ProductSettings>(_mapper.ConfigurationProvider)
            .ToList();
    }
}
