using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using xggameplan.Filters;
using xggameplan.Model;
using xggameplan.Repository;

namespace xggameplan.Controllers
{
    /// <summary>
    /// System messages API
    /// </summary>
    [RoutePrefix("SystemMessages")]
    public class SystemMessagesController : ApiController
    {
        private ISystemMessageRepository _systemMessageRepository;
        private IMapper _mapper;

        public SystemMessagesController(ISystemMessageRepository systemMessageRepository, IMapper mapper)
        {
            _systemMessageRepository = systemMessageRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns all system messages for the specified group
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("SystemMessages")]
        [ResponseType(typeof(List<SystemMessageModel>))]
        public IHttpActionResult GetAll(SystemMessageGroups group)
        {
            var systemMessages = _systemMessageRepository.GetByGroup(group);
            var systemMessagesModel = _mapper.Map<List<SystemMessageModel>>(systemMessages);
            return Ok(systemMessagesModel);
        }
    }
}
