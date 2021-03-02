using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Autopilot.FlexibilityLevels;
using xggameplan.Filters;
using xggameplan.Model;

namespace xggameplan.Controllers
{
    [RoutePrefix("FlexibilityLevels")]
    public class FlexibilityLevelsController : ApiController
    {
        private readonly IFlexibilityLevelRepository _flexibilityLevelRepository;
        private readonly IMapper _mapper;

        public FlexibilityLevelsController(IFlexibilityLevelRepository flexibilityLevelRepository, IMapper mapper)
        {
            _flexibilityLevelRepository = flexibilityLevelRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets all flexibility levels
        /// </summary>
        [Route("")]
        [AuthorizeRequest("AutopilotSettings")]
        [ResponseType(typeof(IEnumerable<FlexibilityLevelModel>))]
        public IEnumerable<FlexibilityLevelModel> GetAll()
        {
            return _mapper.Map<List<FlexibilityLevelModel>>(_flexibilityLevelRepository.GetAll().OrderBy(x => x.SortIndex));
        }
    }
}
