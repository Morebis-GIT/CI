using System;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.System.Products;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class ProductSettingsRepository : IProductSettingsRepository
    {
        private readonly ISqlServerMasterDbContext _dbContext;
        private readonly IMapper _mapper;

        public ProductSettingsRepository(ISqlServerMasterDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void Add(ProductSettings productSettings) => _dbContext.Add(
            _mapper.Map<Entities.Master.ProductSettings>(productSettings),
            post => post.MapTo(productSettings), _mapper);

        public ProductSettings Get(int id) => _mapper.Map<ProductSettings>(
               _dbContext.Find<Entities.Master.ProductSettings>(new object[]
               { id }, post => post.IncludeCollection(e => e.Features)));

        public void SaveChanges() =>
            _dbContext.SaveChanges();

        public void Update(ProductSettings productSettings) =>
            _dbContext.Update(_mapper.Map<Entities.Master.ProductSettings>(productSettings),
                post => post.MapTo(productSettings), _mapper);
    }
}
