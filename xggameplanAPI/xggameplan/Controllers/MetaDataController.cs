using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;
using xggameplan.Errors;
using xggameplan.Filters;

namespace xggameplan.Controllers
{

    /// <summary>
    /// Controller to store and retrieve metadata values like Break Type , program type, Demographic
    /// </summary>
    [RoutePrefix("Metadata")]
    public class MetaDataController : ApiController
    {
        private readonly IMetadataRepository _metadataRepository;
        private readonly IProgrammeCategoryHierarchyRepository _programmeCategoryHierarchyRepository;

        /// <summary>
        /// Initialize metadataRepository instance
        /// </summary>
        /// <param name="metadataRepository"></param>

        public MetaDataController(IMetadataRepository metadataRepository, IProgrammeCategoryHierarchyRepository programmeCategoryHierarchyRepository)
        {
            _metadataRepository = metadataRepository;
            _programmeCategoryHierarchyRepository = programmeCategoryHierarchyRepository;
        }

        /// <summary>
        /// Get metadata value
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("Metadata")]
        public IHttpActionResult Get([FromUri] MetaDataKeys key)
        {
            List<Data> metadata = null;
            if (key == MetaDataKeys.ProgramCategories)
            {
                var programmeCategories = _programmeCategoryHierarchyRepository.GetAll().ToList();
                if (programmeCategories.Any())
                {
                    metadata = new List<Data>(programmeCategories.Select(programmeCategory => new Data {Id = programmeCategory.Id, Value = programmeCategory.Name}));
                }
            }
            else
            {
                metadata =_metadataRepository.GetByKey(key);
            }
            return metadata == null || !metadata.Any()  ? this.NoContent() : Ok(metadata);
        }

        /// <summary>
        /// Save metadata
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("Metadata")]
        public IHttpActionResult Post([FromUri] MetaDataKeys key, [FromBody] List<object> value)
        {
            if (!ModelState.IsValid || value == null)
            {
                return this.Error().InvalidParameters();
            }

            if (key == MetaDataKeys.ProgramCategories)
            {
                _programmeCategoryHierarchyRepository.AddRange(value.Select(m => new ProgrammeCategoryHierarchy { Name = m.ToString() }));
            }
            else
            {
                var dictionaryValue = AppendId(value);
                var metadataModel = _metadataRepository.GetAll() ?? new MetadataModel();

                if (metadataModel.ContainsKey(key))
                {
                    metadataModel[key] = dictionaryValue;
                }
                else
                {
                    metadataModel.Add(key, dictionaryValue);
                }

                _metadataRepository.Add(metadataModel);
                _metadataRepository.SaveChanges();
            }
            return Ok();
        }

        /// <summary>
        /// Delete metadata
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("Metadata")]
        public IHttpActionResult Delete([FromUri] MetaDataKeys key)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters();
            }

            if (key == MetaDataKeys.ProgramCategories)
            {
                _programmeCategoryHierarchyRepository.Truncate();
            }
            else
            {
                var metadataModel = _metadataRepository.GetAll();

                if (metadataModel == null || !metadataModel.Any())
                {
                    return this.NoContent();
                }

                if (metadataModel.ContainsKey(key))
                {
                    metadataModel.Remove(key);
                }

                _metadataRepository.Add(metadataModel);
                _metadataRepository.SaveChanges();
            }

            return Ok();
        }

        /// <summary>
        /// create Id for values
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static List<Data> AppendId(IEnumerable<object> value)
        {
            var count = 0;
            return value.Select(m => new Data() { Id = ++count, Value = m }).ToList();
        }
    }
}
