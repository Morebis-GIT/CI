using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Generic.Validation;
using ImagineCommunications.GamePlan.Domain.Shared.Channels;
using xggameplan.Errors;
using xggameplan.Filters;
using xggameplan.Model;

namespace xggameplan.Controllers
{
    /// <summary>
    /// Channel endpoints
    /// </summary>
    [RoutePrefix("Channels")]
    public class ChannelController : ApiController
    {
        private readonly IChannelsRepository _channelsRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Channel controller constructor
        /// </summary>
        /// <param name="channelsRepository"></param>
        /// <param name="mapper"></param>
        public ChannelController(IChannelsRepository channelsRepository, IMapper mapper)
        {
            _channelsRepository = channelsRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all Channels
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("Channels")]
        public List<ChannelModel> Get()
        {
            var channel = _channelsRepository.GetAll();
            return _mapper.Map<List<ChannelModel>>(channel);
        }

        /// <summary>
        /// Get channel by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [AuthorizeRequest("Channels")]
        [ResponseType(typeof(ChannelModel))]
        public IHttpActionResult Get(int id)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters();
            }
           var channel = _channelsRepository.GetById(id);
           return Ok(_mapper.Map<ChannelModel>(channel));
        }

        /// <summary>
        /// Save channel
        /// </summary>
        /// <param name="command">channel Input value</param>
        [Route("")]
        [AuthorizeRequest("Channels")]
        public IHttpActionResult Post([FromBody] CreateChannelModel command)
        {
            if (command == null || !ModelState.IsValid)
            {
                return this.Error().InvalidParameters();
            }
            ValidateChannel(command.Name,command.ShortName);
            var channel = _mapper.Map<Channel>(command);
            _channelsRepository.Add(channel);
            return Ok();
        }

        private void ValidateChannel(string name, string shortName)
        {
            IValidation validation = new RequiredFieldValidation()
            {
                Field = new List<ValidationInfo>()
                {
                    new ValidationInfo(){FieldName = "Channel Name",FieldToValidate = name},
                    new ValidationInfo(){FieldName = "Channel Short Name",FieldToValidate=shortName}
                }
            };
            validation.Execute();
        }

        /// <summary>
        /// Update the channel
        /// </summary>
        /// <param name="id">Channel id</param>
        /// <param name="command">value to update the Channel</param>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("Channels")]
        public IHttpActionResult Put(int id, [FromBody] CreateChannelModel command)
        {
            if (command == null || !ModelState.IsValid)
            {
                return this.Error().InvalidParameters();
            }
            var channel = _channelsRepository.GetById(id);
            channel = UpdateChannel(channel, command);
            _channelsRepository.Add(channel);
            return Ok();
        }

        /// <summary>
        /// Delete Channel by Id
        /// </summary>
        /// <param name="id">Channel id</param>
        [Route("")]
        [AuthorizeRequest("Channels")]
        public IHttpActionResult Delete(int id)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters();
            }
            _channelsRepository.Delete(id);
            return Ok();
        }

        /// <summary>
        /// update Channel values
        /// </summary>
        /// <param name="channel">existing channel value</param>
        /// <param name="command">input parameter</param>
        /// <returns>channel value</returns>
        private Channel UpdateChannel(Channel channel, CreateChannelModel command)
        {
            channel.Name = command.Name;
            channel.ShortName = command.ShortName;
            return channel;
        }
    }
}
