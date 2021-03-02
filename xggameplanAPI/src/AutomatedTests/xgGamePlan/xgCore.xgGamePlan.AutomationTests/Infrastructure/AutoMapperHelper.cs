using AutoMapper;
using xgCore.xgGamePlan.ApiEndPoints.Models.BRS;

namespace xgCore.xgGamePlan.AutomationTests.Infrastructure
{
    static class AutoMapperHelper
    {
        private readonly static MapperConfiguration _mapperConfiguration;

        static AutoMapperHelper()
        {
            _mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<BRSConfigurationTemplateModel, CreateOrUpdateBRSConfigurationTemplateModel>();
            });
        }

        public static IMapper GetMapper()
        {
            return new Mapper(_mapperConfiguration);
        }
    }
}
