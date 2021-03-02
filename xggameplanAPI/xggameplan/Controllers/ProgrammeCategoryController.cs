using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using xggameplan.Errors;
using xggameplan.Filters;
using xggameplan.model.External;

namespace xggameplan.Controllers
{
    [RoutePrefix("ProgrammeCategories")]
    public class ProgrammeCategoryController : ApiController
    {
        private readonly IProgrammeCategoryHierarchyRepository _programmeCategoryHierarchyRepository;
        private readonly IMapper _mapper;

        public ProgrammeCategoryController(IProgrammeCategoryHierarchyRepository programmeCategoryHierarchyRepository, IMapper mapper)
        {
            _programmeCategoryHierarchyRepository = programmeCategoryHierarchyRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all programme categories
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("ProgrammeCategories")]
        [ResponseType(typeof(IEnumerable<ProgrammeCategoryHierarchyModel>))]
        public IEnumerable<ProgrammeCategoryHierarchyModel> GetAll()
        {
            var programmeCategories = _programmeCategoryHierarchyRepository.GetAll().ToList();
            return _mapper.Map<List<ProgrammeCategoryHierarchyModel>>(programmeCategories);
        }

        /// <summary>
        /// Create a list of new programme categories
        /// </summary>
        /// <param name="programmeCategories"></param>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("ProgrammeCategories")]
        public IHttpActionResult Post([FromBody] List<ProgrammeCategoryHierarchyModel> programmeCategories)
        {
            if (programmeCategories is null || !programmeCategories.Any())
            {
                return this.Error().InvalidParameters();
            }

            programmeCategories.ForEach(c => c.Id = 0);

            _programmeCategoryHierarchyRepository.AddRange(_mapper.Map<List<ProgrammeCategoryHierarchy>>(programmeCategories));
            _programmeCategoryHierarchyRepository.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Deletes all programme categories
        /// </summary>
        /// <returns></returns>
        [Route("DeleteAll")]
        [AuthorizeRequest("ProgrammeCategories")]
        public IHttpActionResult Delete()
        {
            var programmeCategories = _programmeCategoryHierarchyRepository.GetAll();

            if (!programmeCategories.Any())
            {
                return this.NoContent();
            }

            _programmeCategoryHierarchyRepository.Truncate();
            _programmeCategoryHierarchyRepository.SaveChanges();
            return Ok();
        }
    }
}
