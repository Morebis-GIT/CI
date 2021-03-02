using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.ProgrammeClassifications;
using xggameplan.Errors;
using xggameplan.Filters;
using xggameplan.Model;

namespace xggameplan.Controllers
{
    /// <summary>
    /// Programme Classifications API
    /// </summary>
    [RoutePrefix("Programmes/Classifications")]
    public class ProgrammeClassificationsController : ApiController
    {
        private readonly IProgrammeClassificationRepository _programmeClassificationRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Programme Classification controller constructor
        /// </summary>
        /// <param name="programmeClassificationRepository"></param>
        /// <param name="mapper"></param>
        public ProgrammeClassificationsController(IProgrammeClassificationRepository programmeClassificationRepository, IMapper mapper)
        {
            _programmeClassificationRepository = programmeClassificationRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all Programme Classifications
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("ProgrammeClassifications")]
        public List<ProgrammeClassificationModel> Get()
        {
            var d = _programmeClassificationRepository.GetAll().OrderBy(pc => pc.Uid);
            return _mapper.Map<List<ProgrammeClassificationModel>>(d);
        }

        /// <summary>
        /// Get Programme Classification by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [AuthorizeRequest("ProgrammeClassifications")]
        [ResponseType(typeof(ProgrammeClassificationModel))]
        public IHttpActionResult Get(int id)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters();
            }
            var pc = _programmeClassificationRepository.GetById(id);
            if (pc == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<ProgrammeClassificationModel>(pc));
        } 

        /// <summary>
        /// Creates Programme Classifications
        /// </summary>
        /// <param name="commands">Demographics</param>
        [Route("")]
        [AuthorizeRequest("ProgrammeClassifications")]
        public IHttpActionResult Post([FromBody] List<ProgrammeClassificationModel> commands)
        {
            if (commands == null || !commands.Any() || !ModelState.IsValid)
            {
                return this.Error().InvalidParameters();
            }

            // Check unique values are unique in the batch being posted
            if (commands.Select(c => c.Uid).Distinct().ToList().Count != commands.Count)
            {
                return this.Error().InvalidParameters("Uid must be unique");
            }
            if (commands.Select(c => c.Code).Distinct().ToList().Count != commands.Count)
            {
                return this.Error().InvalidParameters("Code must be unique");
            }

            // Create list of programme classifications, perform validation
            var allProgrammeClassifications = _programmeClassificationRepository.GetAll();
            var programmeClassifications = new List<ProgrammeClassification>();
            foreach (var command in commands)
            {
                var programmeClassification = _mapper.Map<ProgrammeClassification>(command);
                var validation = ValidateForSave(programmeClassification, allProgrammeClassifications);           
                if (validation != null)
                {
                    return validation;
                }
                programmeClassifications.Add(programmeClassification);
            }

            _programmeClassificationRepository.Add(programmeClassifications);
            return Ok();
        }

        private IHttpActionResult ValidateForSave(ProgrammeClassification programmeClassification, IEnumerable<ProgrammeClassification> allProgrammeClassifications)
        {
            if (programmeClassification.Uid <= 0)   // While property is called Uid then it isn't auto-generated
            {
                return this.Error().InvalidParameters("Uid is invalid");
            }
            if (string.IsNullOrEmpty(programmeClassification.Code))
            {
                return this.Error().InvalidParameters("Code is not set");
            }
            if (string.IsNullOrEmpty(programmeClassification.Description))
            {
                return this.Error().InvalidParameters("Description is not set");
            }

            // Check unique-ness, in preparation for Raven 4 then we don't use unique constraints
            if (allProgrammeClassifications.FirstOrDefault(c => c.Uid == programmeClassification.Uid && c.Uid != programmeClassification.Uid) != null)
            {                
                return this.Error().InvalidParameters("Uid must be unique");
            }
            if (allProgrammeClassifications.FirstOrDefault(c => c.Code == programmeClassification.Code && c.Uid != programmeClassification.Uid) != null)
            {
                return this.Error().InvalidParameters("Code must be unique");
            }
            if (allProgrammeClassifications.FirstOrDefault(c => c.Description.ToLower() == programmeClassification.Description.ToLower() && c.Uid != programmeClassification.Uid) != null)
            {
                return this.Error().InvalidParameters("Description must be unique");
            }
            return null;
        }

        /// <summary>
        /// Update the programme classification
        /// </summary>
        [Route("")]
        [AuthorizeRequest("ProgrammeClassifications")]
        public IHttpActionResult Put(int id, [FromBody] ProgrammeClassificationModel command)
        {
            if (command == null || !ModelState.IsValid)
            {
                return this.Error().InvalidParameters();
            }

            var allProgrammeClassifications = _programmeClassificationRepository.GetAll();
            var pc = allProgrammeClassifications.FirstOrDefault(c => c.Uid == id);
            if (pc == null)
            {
                return NotFound();
            }

            pc.Uid = id;
            pc.Code = command.Code;
            pc.Description = command.Description;
            var validation = ValidateForSave(pc, allProgrammeClassifications);     
            if (validation != null)
            {
                return validation;
            }

            _programmeClassificationRepository.Update(pc);
            return Ok();
        }

        /// <summary>
        /// Delete programme classification by Id
        /// </summary>
        /// <param name="id">Id</param>
        [Route("")]
        [AuthorizeRequest("ProgrammeClassifications")]
        public IHttpActionResult Delete(int id)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters();
            }
            var pc = _programmeClassificationRepository.GetById(id);
            if (pc == null)
            {
                return this.NotFound();
            }
            _programmeClassificationRepository.Delete(id);
            return Ok();
        }

        /// <summary>
        /// Deletes all programme classifications
        /// </summary>
        [Route("DeleteAll")]
        [AuthorizeRequest("ProgrammeClassifications")]
        public IHttpActionResult Delete()
        {
            _programmeClassificationRepository.Truncate();
            return Ok();
        }
    }
}
