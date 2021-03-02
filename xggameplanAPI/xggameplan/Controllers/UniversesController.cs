using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Validation;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using xggameplan.Errors;
using xggameplan.Extensions;
using xggameplan.Filters;
using xggameplan.Model;
using xggameplan.Services;

namespace xggameplan.Controllers
{
    [RoutePrefix("Universes")]
    public class UniversesController : ApiController
    {
        private readonly IUniverseRepository _universeRepository;
        private readonly IDemographicRepository _demographicRepository;
        private readonly ISalesAreaRepository _salesAreaRepository;
        private readonly IDataChangeValidator _dataChangeValidator;
        private readonly IMapper _mapper;

        public UniversesController(IUniverseRepository universeRepository, IDemographicRepository demographicRepository,
            ISalesAreaRepository salesAreaRepository,
            IDataChangeValidator dataChangeValidator, IMapper mapper)
        {
            _universeRepository = universeRepository;
            _demographicRepository = demographicRepository;
            _salesAreaRepository = salesAreaRepository;
            _dataChangeValidator = dataChangeValidator;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all universes
        /// </summary>
        [Route("")]
        [AuthorizeRequest("Universes")]
        [ResponseType(typeof(IEnumerable<UniverseModel>))]
        public IEnumerable<UniverseModel> Get()
        {
            var universes = _universeRepository.GetAll();
            return _mapper.Map<List<UniverseModel>>(universes);
        }

        /// <summary>
        /// Gets a single universe
        /// </summary>
        /// <param name="id"></param>
        [Route("{id}")]
        [AuthorizeRequest("Universes")]
        [ResponseType(typeof(UniverseModel))]
        public IHttpActionResult Get(Guid id)
        {
            var item = _universeRepository.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<UniverseModel>(item));
        }

        [Route("Search")]
        [AuthorizeRequest("Universes")]
        [ResponseType(typeof(IEnumerable<UniverseModel>))]
        [HttpGet]
        public IEnumerable<UniverseModel> Search([FromUri] DateTime? startDate = null, [FromUri] DateTime? endDate = null, [FromUri] List<string> demographics = null, [FromUri] List<string> salesAreas = null)
        {
            var fromDate = startDate?.ToUniversalTime().Date ?? DateTime.MinValue;
            var toDate = endDate?.ToUniversalTime().Date ?? DateTime.MinValue;

            var universes = _universeRepository.Search(demographics, salesAreas, fromDate, toDate);
            if (universes != null && universes.Any())
            {
                return _mapper.Map<List<UniverseModel>>(universes);
            }

            return null;
        }

        public void ValidateUniverse(List<CreateUniverse> commands)
        {
            commands.ForEach(command => Validate(command.SalesArea, command.Demographic, command.StartDate,
                command.EndDate, command.UniverseValue));

            _salesAreaRepository.ValidateSaleArea(commands.Select(_ => _.SalesArea).ToList());
            if (!_demographicRepository.ValidateDemographics(commands.Select(u => u.Demographic).ToList(),
                out List<string> invalidDemographics))
            {
                var msg = string.Concat("Invalid Demographics: ",
                    invalidDemographics != null ? string.Join(",", invalidDemographics) : string.Empty);
                throw new InvalidDataException(msg);
            }
        }

        private void Validate(string salesArea, string demographic, DateTime startDate, DateTime endDate,
            int universeValue)

        {
            IValidation validation = new RequiredFieldValidation()
            {
                Field = new List<ValidationInfo>()
                {
                    new ValidationInfo() {FieldName = "Sales Area", FieldToValidate = salesArea},
                    new ValidationInfo() {FieldName = "Demographic", FieldToValidate = demographic},
                    new ValidationInfo() {FieldName = "Start Date", FieldToValidate = startDate},
                    new ValidationInfo() {FieldName = "End Date", FieldToValidate = endDate},
                    new ValidationInfo() {FieldName = "Universe Value", FieldToValidate = universeValue}
                }
            };
            validation.Execute();

            validation = new CompareValidation()
            {
                Field = new List<ValidationInfo>()
                {
                    new ValidationInfo()
                    {
                        ErrorMessage = "Universe start date should be less than or equal to end date",
                        FieldToValidate = startDate,
                        FieldToCompare =endDate,
                        Operator = Operator.LessThanEqual
                    }
                }
            };
            validation.Execute();
        }

        /// <summary>
        /// Deletes Universe
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [AuthorizeRequest("Universes")]
        public IHttpActionResult Delete(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters("Invalid parameters");
            }

            var universe = _universeRepository.Find(id);
            if (universe == null)
            {
                return this.NotFound();
            }
            _universeRepository.Remove(id);
            return this.NoContent();
        }

        /// <summary>
        /// Deletes Universe based on the input parameters
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="salesArea"></param>
        /// <param name="demographic"></param>
        /// <returns></returns>
        [Route("Delete")]
        [AuthorizeRequest("Universes")]
        public IHttpActionResult Delete([FromUri] DateTime? startDate = null, DateTime? endDate = null, string salesArea = null, string demographic = null)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters("Invalid parameters");
            }
            if (salesArea == null && demographic == null && startDate == null && endDate == null)
            {
                return this.Error().InvalidParameters("At least one of the parameters must be specified");
            }
            if ((startDate != null && endDate == null) || (startDate == null && endDate != null))
            {
                return this.Error().InvalidParameters("For date range based deletion, both start and end dates must be provided or both can be null");
            }
            if (startDate != null && endDate != null)
            {
                if (startDate > endDate)
                {
                    return this.Error().InvalidParameters("Start date cannot be greater than end date");
                }
            }
            _universeRepository.DeleteByCombination(salesArea, demographic, startDate?.ToUniversalTime(), endDate?.ToUniversalTime());
            return Ok();
        }

        /// <summary>
        /// Deletes all Universes
        /// </summary>
        [Route("DeleteAll")]
        [AuthorizeRequest("Universes")]
        public IHttpActionResult Delete()
        {
            // Validate that we can delete
            _dataChangeValidator.ThrowExceptionIfAnyErrors(_dataChangeValidator.ValidateChange<Universe>(ChangeActions.Delete, ChangeTargets.AllItems, null));

            _universeRepository.Truncate();
            return Ok();
        }

        /// <summary>
        /// Creates single/multiple universe instances
        /// </summary>
        /// <param name="commands"></param>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("Universes")]
        public IHttpActionResult Post([FromBody] List<CreateUniverse> commands)
        {
            if (commands == null || !commands.Any() || !ModelState.IsValid)
            {
                return this.Error().InvalidParameters("Invalid Universe parameters");
            }

            commands.ForEach(command =>
            {
                command.Id = Guid.NewGuid();
                if (command.EndDate == DateTime.MinValue)
                {
                    command.EndDate = command.StartDate.AddYears(2);
                }
                // remove time value . Only Date Without time
                command.EndDate = command.EndDate.Date;
                command.StartDate = command.StartDate.Date;
            });
            ValidateUniverse(commands);

            var universes = _universeRepository.Search(commands.Select(_ => _.Demographic).Distinct().ToList(), commands.Select(_ => _.SalesArea).Distinct().ToList(), DateTime.MinValue, DateTime.MinValue).ToList();
            SaveUniverse(commands, universes);
            return Ok();
        }

        private void SaveUniverse(List<CreateUniverse> newUniverses, List<Universe> existingUniverses)
        {
            var universes = new List<Universe>();
            newUniverses.OrderBy(d => d.StartDate).GroupBy(g => new { Demographic = g.Demographic.ToUpper(), Salesarea = g.SalesArea }).ToList().ForEach(
                g =>
                {
                    var existingUniverse = existingUniverses?.Where(
                         _ => _.Demographic.Equals(g.Key.Demographic, StringComparison.OrdinalIgnoreCase) &&
                              _.SalesArea.Equals(g.Key.Salesarea))?.OrderByDescending(_ => _.EndDate)?.FirstOrDefault();

                    g.ToList().ForEach(item =>
                    {
                        SaveUniverse(item, existingUniverse, out Universe nextUniverse, ref universes);
                        existingUniverse = nextUniverse;
                    });
                });
            if (universes.Any())
            {
                _universeRepository.Insert(universes);
            }
        }

        private void SaveUniverse(CreateUniverse command, Universe lastUniverse, out Universe nextUniverse, ref List<Universe> universes)
        {
            nextUniverse = null;
            if (lastUniverse == null)
            {
                nextUniverse = _mapper.Map<Universe>(command);
                AddorUpdate(ref universes, nextUniverse); // "Add"
                return;
            }

            if (command.StartDate < lastUniverse.StartDate.Date)
            {
                throw new Exception("universe start date(" + command.StartDate +
                                    ") should be greater than or equal to existing universe start date(" +
                                    lastUniverse.StartDate + "). Sales area:" + command.SalesArea + ", Demographic:" + command.Demographic);
            }

            if (command.StartDate == lastUniverse.StartDate.Date) //"update/overwrite";
            {
                lastUniverse.EndDate = command.EndDate;
                lastUniverse.UniverseValue = command.UniverseValue;
                nextUniverse = lastUniverse;
                AddorUpdate(ref universes, lastUniverse);
                return;
            }

            if (command.StartDate > lastUniverse.StartDate && command.EndDate <= lastUniverse.EndDate)
            {
                throw new Exception(
                    "universe start and end date range should not fully overlap with existing universe start date(" +
                    lastUniverse.StartDate + ") end date (" + lastUniverse.EndDate + "). Sales area:" + command.SalesArea + ", Demographic:" + command.Demographic);
            }

            if (command.StartDate == lastUniverse.EndDate.AddDays(1).Date)
            {
                nextUniverse = _mapper.Map<Universe>(command);
                AddorUpdate(ref universes, nextUniverse); // "Add"
                return;
            }

            if (command.StartDate > lastUniverse.EndDate.AddDays(1).Date)
            {
                throw new Exception("Universe start date should not be greater than(" +
                                    lastUniverse.EndDate.AddDays(1).Date + "). Sales area:" + command.SalesArea + ", Demographic:" + command.Demographic);
            }

            if (command.StartDate > lastUniverse.StartDate.Date && command.StartDate <= lastUniverse.EndDate.Date &&
                command.EndDate > lastUniverse.EndDate.Date
            ) //   "Add it as new and also update the existing start date";
            {
                nextUniverse = _mapper.Map<Universe>(command);
                AddorUpdate(ref universes, nextUniverse);

                lastUniverse.EndDate = command.StartDate.AddDays(-1).Date;
                AddorUpdate(ref universes, lastUniverse);
            }
        }

        private static void AddorUpdate(ref List<Universe> universes, Universe universe)
        {
            universes.RemoveAll(_ => _.Id.Equals(universe.Id));
            universes.Add(universe);
        }
    }
}
