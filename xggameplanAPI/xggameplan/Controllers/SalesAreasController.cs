using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using xggameplan.Errors;
using xggameplan.Extensions;
using xggameplan.Filters;
using xggameplan.Model;

namespace xggameplan.Controllers
{
    [RoutePrefix("SalesAreas")]
    public class SalesAreasController : ApiController
    {
        private readonly IDemographicRepository _demographicRepository;
        private readonly ISalesAreaRepository _salesAreaRepository;
        private readonly ISalesAreaCleanupDeleteCommand _cleanupDeleteCommand;
        private readonly IMapper _mapper;

        public SalesAreasController(
            IDemographicRepository demographicRepository,
            ISalesAreaRepository salesAreaRepository,
            ISalesAreaCleanupDeleteCommand salesAreaCleanupDeleteCommand,
            IMapper mapper)
        {
            _demographicRepository = demographicRepository;
            _salesAreaRepository = salesAreaRepository;
            _cleanupDeleteCommand = salesAreaCleanupDeleteCommand;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all SalesAreas
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("SalesAreas")]
        public IEnumerable<SalesAreaModel> Get()
        {
            var salesAreas = _salesAreaRepository.GetAll().ToList();
            return _mapper.Map<List<SalesAreaModel>>(salesAreas);
        }

        /// <summary>
        /// Get SalesArea by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [AuthorizeRequest("SalesAreas")]
        [ResponseType(typeof(SalesAreaModel))]
        public IHttpActionResult Get(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters();
            }

            var salesArea = _salesAreaRepository.Find(id);
            return Ok(_mapper.Map<SalesAreaModel>(salesArea));
        }

        /// <summary>
        /// Save salesarea
        /// </summary>
        /// <param name="command">SalesArea input values</param>
        [Route("")]
        [AuthorizeRequest("SalesAreas")]
        public IHttpActionResult Post([FromBody] CreateSalesAreaModel command)
        {
            if (command == null || !ModelState.IsValid)
            {
                return this.Error().InvalidParameters();
            }
            Validate(command.BaseDemographic1, command.BaseDemographic2);

            //Input values are validated by calling Validate method as part of MapFrom
            var salesArea = SalesArea.MapFrom(Guid.NewGuid(),
                command.Name,
                command.ShortName,
                command.CurrencyCode,
                command.BaseDemographic1,
                command.BaseDemographic2,
                command.ChannelGroup,
                command.StartOffset,
                command.DayDuration);
            _salesAreaRepository.Add(salesArea);
            _salesAreaRepository.SaveChanges();
            return Ok(_mapper.Map<SalesAreaModel>(salesArea));
        }

        private void Validate(params string[] demographic)
        {
            if (_demographicRepository.ValidateDemographics(demographic.ToList(), out List<string> invaliddemographic))
            {
                return;
            }

            var msg = string.Concat("Invalid demographic number: ", string.Join(",", invaliddemographic));
            throw new InvalidDataException(msg);
        }

        /// <summary>
        /// Update the SalesArea
        /// </summary>
        /// <param name="id">SalesArea Id</param>
        /// <param name="command">SalesArea input values</param>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("SalesAreas")]
        public IHttpActionResult Put(Guid id, [FromBody] CreateSalesAreaModel command)
        {
            if (command == null || !ModelState.IsValid)
            {
                return this.Error().InvalidParameters();
            }

            var salesArea = _salesAreaRepository.Find(id);
            if (salesArea == null)
            {
                return NotFound();
            }

            Validate(command.BaseDemographic1, command.BaseDemographic2);

            //Input values are validated by calling Validate method as part of Update
            salesArea.Update(id,
                command.Name,
                command.ShortName,
                command.CurrencyCode,
                command.BaseDemographic1,
                command.BaseDemographic2,
                command.ChannelGroup,
                command.StartOffset,
                command.DayDuration);
            _salesAreaRepository.Update(salesArea);

            return Ok(_mapper.Map<SalesAreaModel>(salesArea));
        }

        /// <summary>
        /// Delete SalesArea by Id
        /// </summary>
        /// <param name="id">SalesArea id</param>
        [Route("")]
        [AuthorizeRequest("SalesAreas")]
        public IHttpActionResult Delete(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters();
            }

            _cleanupDeleteCommand.Execute(id);

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
