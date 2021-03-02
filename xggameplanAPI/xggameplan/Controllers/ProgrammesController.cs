using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Generic.SequenceCounter;
using ImagineCommunications.GamePlan.Domain.Generic.Validation;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Counters;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Queries;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using NodaTime;
using Raven.Abstractions.Extensions;
using xggameplan.common.Services;
using xggameplan.Errors;
using xggameplan.Extensions;
using xggameplan.Filters;
using xggameplan.Model;
using xggameplan.Services;

namespace xggameplan.Controllers
{
    [RoutePrefix("Programmes")]
    public class ProgrammesController : ApiController
    {
        private readonly IProgrammeRepository _programmeRepository;
        private readonly IMapper _mapper;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IProgrammeCategoryHierarchyRepository _programmeCategoryRepository;
        private readonly ISalesAreaRepository _salesAreaRepository;
        private readonly IDataChangeValidator _dataChangeValidator;

        public ProgrammesController(IProgrammeRepository programmeRepository, IMapper mapper,
            IRepositoryFactory repositoryFactory,
            IProgrammeCategoryHierarchyRepository programmeCategoryRepository,
            ISalesAreaRepository salesAreaRepository, IDataChangeValidator dataChangeValidator)
        {
            _programmeRepository = programmeRepository;
            _mapper = mapper;
            _repositoryFactory = repositoryFactory;
            _programmeCategoryRepository = programmeCategoryRepository;
            _salesAreaRepository = salesAreaRepository;
            _dataChangeValidator = dataChangeValidator;
        }

        /// <summary>
        /// Gets all program
        /// </summary>
        [Route("")]
        [AuthorizeRequest("Programmes")]
        [ResponseType(typeof(IEnumerable<ProgrammeModel>))]
        public IEnumerable<ProgrammeModel> Get()
        {
            var programmes = _programmeRepository.GetAll();
            if (programmes != null && programmes.Any())
            {
                return _mapper.Map<List<ProgrammeModel>>(programmes.ToList());
            }

            return null;
        }

        [Route("SearchAll")]
        [AuthorizeRequest("Programmes")]
        [ResponseType(typeof(SearchResultModel<ProgrammeNameModel>))]
        public IHttpActionResult Get([FromUri] ProgrammeSearchQueryModel queryModel)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters("One or more of the required query parameters are missing.");
            }
            if (queryModel == null)
            {
                queryModel = new ProgrammeSearchQueryModel();
            }
            var programmes = _programmeRepository.Search(queryModel);

            var searchModel = new SearchResultModel<ProgrammeNameModel>()
            {
                Items = programmes.Items.ToList(),
                TotalCount = programmes.TotalCount
            };
            return Ok(searchModel);
        }

        /// <summary>
        /// Gets a range of Program
        /// </summary>
        /// <param name="datefrom"></param>
        /// <param name="dateto"></param>
        /// <param name="salesarea"></param>
        [Route("Search")]
        [AuthorizeRequest("Programmes")]
        [ResponseType(typeof(IEnumerable<ProgrammeModel>))]
        public IHttpActionResult Get([FromUri] DateTime datefrom,
            [FromUri] DateTime dateto,
            [FromUri] string salesarea)
        {
            var programmes = _programmeRepository.Search(datefrom, dateto, salesarea);
            if (programmes != null && programmes.Any())
            {
                return Ok(_mapper.Map<List<ProgrammeModel>>(programmes.ToList()));
            }

            return this.NoContent();
        }

        /// <summary>
        /// Creates a set of Programmes
        /// </summary>
        [Route("")]
        [AuthorizeRequest("Programmes")]
        public IHttpActionResult Post([FromBody] List<CreateProgramme> programs)
        {
            if (programs == null || !programs.Any() || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ValidatePrograms(programs);

            var programmeList = _mapper.Map<IEnumerable<Programme>>(programs)
                .OrderBy(x => x.StartDateTime)
                .SequentiallyCount<Programme>(new ProgrammePrgtNoSequenceCounter())
                .ToList();

            _programmeRepository.Add(programmeList);

            // group by date and channels
            programmeList.GroupBy(s => new { s.StartDateTime.Date, s.SalesArea }).ForEach(grp =>
              {
                  using (MachineLock.Create(string.Format("xggameplan.scheduleday.{0}.{1}", grp.Key.SalesArea, grp.Key.Date), new TimeSpan(0, 10, 0)))
                  {
                      using (var scope = _repositoryFactory.BeginRepositoryScope())
                      {
                          var scheduleRepository = scope.CreateRepository<IScheduleRepository>();
                          var schedule = scheduleRepository.GetOrCreateSchedule(grp.Key.SalesArea, grp.Key.Date);

                          if (schedule.Programmes is null)
                          {
                              schedule.Programmes = new List<Programme>();
                          }

                          schedule.Programmes.AddRange(grp);
                          scheduleRepository.Add(schedule);
                          scheduleRepository.SaveChanges();
                      }
                  }
              });

            return Ok();
        }

        private void ValidatePrograms(List<CreateProgramme> programs)
        {
            //Required field validation
            programs.ForEach(b =>
            {
                Validate(b.SalesArea, b.StartDateTime, b.Duration, b.ExternalReference, b.ProgrammeName);
            });
            var programcategorysList = programs.Where(p => p.ProgrammeCategories != null)
                .SelectMany(b => b.ProgrammeCategories)
                .Where(p => !string.IsNullOrWhiteSpace(p)).ToList();
            //Validation for category type
            if (programcategorysList.Count > 0 && !_programmeCategoryRepository.IsValid(programcategorysList, out List<string> invaliddata))
            {
                var msg = "category names are invalid";
                if (invaliddata == null || invaliddata.Count <= 0)
                {
                    throw new InvalidDataException(msg);
                }

                msg = msg + ": " + string.Join(",", invaliddata);
                throw new InvalidDataException(msg);
            }

            _salesAreaRepository.ValidateSaleArea(programs.Select(s => s.SalesArea).ToList());
        }

        private void Validate(string salesArea, DateTime startDateTime, Duration duration, string externalReference, string programName)
        {
            IValidation validation = new RequiredFieldValidation()
            {
                Field = new List<ValidationInfo>()
                {
                    new ValidationInfo(){FieldName = "Sales Area",FieldToValidate = salesArea},
                    new ValidationInfo(){FieldName ="Start Date Time",FieldToValidate=startDateTime} ,
                    new ValidationInfo(){FieldName ="Duration",FieldToValidate=duration},
                    new ValidationInfo(){FieldName ="External Reference",FieldToValidate=externalReference},
                    new ValidationInfo(){FieldName ="Program Name",FieldToValidate=programName}
                }
            };
            validation.Execute();
        }

        /// <summary>
        /// Deletes a range of Programs
        /// </summary>
        /// <param name="datefrom"></param>
        /// <param name="dateto"></param>
        /// <param name="salesarea"></param>
        [Route("Delete")]
        [AuthorizeRequest("Programmes")]
        public IHttpActionResult Delete([FromUri] DateTime datefrom,
                                        [FromUri] DateTime dateto,
                                        [FromUri] string salesarea)
        {
            var items = _programmeRepository.Search(datefrom, dateto, salesarea);

            foreach (var p in items)
            {
                _programmeRepository.Remove(p.Id);
            }
            _programmeRepository.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Delete all Programmes and Programme Dictionaries
        /// </summary>
        [Route("DeleteAll")]
        [AuthorizeRequest("Programmes")]
        public async Task<IHttpActionResult> DeleteAsync()
        {
            // Validate that we can delete
            _dataChangeValidator.ThrowExceptionIfAnyErrors(
                _dataChangeValidator.ValidateChange<Programme>(
                    ChangeActions.Delete,
                    ChangeTargets.AllItems,
                    null
                    ));

            await _programmeRepository.TruncateAsync();

            return Ok();
        }
    }
}
