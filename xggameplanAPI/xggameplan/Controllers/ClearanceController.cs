using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.ClearanceCodes;
using xggameplan.Errors;
using xggameplan.Filters;
using xggameplan.Model;

namespace xggameplan.Controllers
{
    [RoutePrefix("ClearanceCode")]
    public class ClearanceController : ApiController
    {
        private readonly IClearanceRepository _clearanceRepository;
        private IMapper _mapper;

        public ClearanceController(IClearanceRepository clearanceRepository, IMapper mapper)
        {
            _clearanceRepository = clearanceRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all Clearance Code
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("ClearanceCode")]
        [ResponseType(typeof(IEnumerable<ClearanceCodeModel>))]
        public IEnumerable<ClearanceCodeModel> Get()
        {
            var clearanceCodes = _clearanceRepository.GetAll()?.ToList();
            if (clearanceCodes != null && clearanceCodes.Any())
            {
                return _mapper.Map<List<ClearanceCodeModel>>(clearanceCodes);
            }

            return null;
        }

        /// <summary>
        /// Get a Clearance Code
        /// </summary>
        /// <returns></returns>
        [Route("{id}")]
        [AuthorizeRequest("ClearanceCode")]
        [ResponseType(typeof(ClearanceCodeModel))]
        public IHttpActionResult Get(int id)
        {
            if (id == 0)
            {
                return this.Error().InvalidParameters();
            }
            var clearancecode = _clearanceRepository.Find(id);
            if (clearancecode == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<ClearanceCodeModel>(clearancecode));
        }

        /// <summary>
        /// Creates a set of clearance Code
        /// </summary>
        [Route("")]
        [AuthorizeRequest("ClearanceCode")]
        [ResponseType(typeof(IEnumerable<ClearanceCodeModel>))]
        public IHttpActionResult Post([FromBody] IList<CreateClearanceCode> commands)
        {
            if (commands == null || !commands.Any() || !ModelState.IsValid)
            {
                return this.Error().InvalidParameters();
            }
            var clearanceCodes = _mapper.Map<List<ClearanceCode>>(commands);
            ValidateForSave(clearanceCodes);
            _clearanceRepository.Add(clearanceCodes);
            return Ok(_mapper.Map<List<ClearanceCodeModel>>(clearanceCodes));
        }

        /// <summary>
        /// Update a clearance Code
        /// </summary>
        [Route("{id}")]
        [AuthorizeRequest("ClearanceCode")]
        [ResponseType(typeof(ClearanceCodeModel))]
        public IHttpActionResult Put(int id, [FromBody] CreateClearanceCode command)
        {
            if (command == null || !ModelState.IsValid || id == 0)
            {
                return this.Error().InvalidParameters();
            }

            var clearancecode = _clearanceRepository.Find(id);
            if (clearancecode == null)
            {
                return NotFound();
            }
            ClearanceCode.Validate(command.Code, command.Description);
            var clearances = _clearanceRepository.FindByExternal(command.Code).ToList();
            //No duplicate clearance code
            if (clearances.Count != 0 && (clearances.Count != 1 || clearances[0].Id != id))
            {
                return this.Error().InvalidParameters("Clearance code already exists");
            }

            MapTo(command, ref clearancecode);
            _clearanceRepository.Add(clearancecode);
            _clearanceRepository.SaveChanges();

            return Ok(_mapper.Map<ClearanceCodeModel>(clearancecode));
        }

        /// <summary>
        /// Delete a clearance Code
        /// </summary>
        [Route("")]
        [AuthorizeRequest("ClearanceCode")]
        public IHttpActionResult Delete(int id)
        {
            if (id == 0)
            {
                return this.Error().InvalidParameters();
            }

            _clearanceRepository.Remove(id, out bool isDeleted);
            if (isDeleted)
            {
                return Ok();
            }

            return NotFound();
        }

        /// <summary>
        /// Deletes all Clearance Code
        /// </summary>
        [Route("DeleteAll")]
        [AuthorizeRequest("ClearanceCode")]
        public IHttpActionResult Delete()
        {
            _clearanceRepository.Truncate();

            return Ok();
        }

        private void MapTo(CreateClearanceCode command, ref ClearanceCode clearancecode)
        {
            clearancecode.Description = command.Description;
            clearancecode.Code = command.Code;
        }

        private void ValidateForSave(List<ClearanceCode> clearanceCodes)
        {
            clearanceCodes.ForEach(c => ClearanceCode.Validate(c.Code, c.Description));

            var clearanceInDb = _clearanceRepository.FindByExternal(clearanceCodes.Select(c => c.Code)?.ToList())?.ToList();
            if (clearanceInDb != null && clearanceInDb.Any())
            {
                var msg = "Clearances code already exists: " +
                          string.Join(",", clearanceInDb.Select(c => c.Code)?.Distinct().ToList());
                throw new InvalidDataException(msg);
            }
        }
    }
}
