using System;
using AutoMapper;

namespace xggameplan.core.Configuration
{
    public static class AutoMapperInitializer
    {
        /// <summary>
        /// Initializes mapper
        /// </summary>
        /// <param name="initializers">list of methods for loading profiles</param>
        public static IMapper Initialize(params Action<IMapperConfigurationExpression>[] initializers)
        {
            var config = new MapperConfiguration(configuration =>
            {
                foreach (var initializer in initializers)
                {
                    initializer(configuration);
                }
            });

            var mapper = new Mapper(config);

            return mapper;
        }
    }
}
