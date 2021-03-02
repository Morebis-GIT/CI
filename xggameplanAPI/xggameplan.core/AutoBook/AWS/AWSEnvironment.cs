using AutoMapper;
using xggameplan.AutoBooks.Abstractions;

namespace xggameplan.AutoBooks.AWS
{
    /// <summary>
    /// For managing Autobooks Environment
    /// </summary>
    public class AWSEnvironment : IEnvironment
    {
        private readonly IEnvironmentAPI<AWSPAEnvironment> _environmentsApi;
        private readonly IMapper _mapper;

        public AWSEnvironment(IEnvironmentAPI<AWSPAEnvironment> environmentsApi, IMapper mapper)
        {
            _environmentsApi = environmentsApi;
            _mapper = mapper;
        }

        public Environment Get()
        {
            var environmentObject = _environmentsApi.Get();
            return _mapper.Map<Environment>(environmentObject);
        }

    }
}
