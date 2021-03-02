using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Queries;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Validation;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Domain.Shared.ClearanceCodes;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using xggameplan.Errors;
using xggameplan.Extensions;
using xggameplan.Filters;
using xggameplan.Model;

namespace xggameplan.Controllers
{
    [RoutePrefix("restrictions")]
    public class RestrictionController : ApiController
    {
        private readonly IRestrictionRepository _restrictionRepository;
        private readonly ISalesAreaRepository _salesAreaRepository;
        private readonly IClearanceRepository _clearanceRepository;
        private readonly IProgrammeCategoryHierarchyRepository _programmeCategoryRepository;
        private readonly IMapper _mapper;

        public RestrictionController(IRestrictionRepository restrictionRepository,
            ISalesAreaRepository salesAreaRepository, IClearanceRepository clearanceRepository, IProgrammeCategoryHierarchyRepository programmeCategoryRepository, IMapper mapper)
        {
            _restrictionRepository = restrictionRepository;
            _salesAreaRepository = salesAreaRepository;
            _clearanceRepository = clearanceRepository;
            _programmeCategoryRepository = programmeCategoryRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a Set of Restriction
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("Restrictions")]
        public IHttpActionResult Post([FromBody] List<CreateRestriction> command)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters();
            }

            Restriction restriction = null;
            var returnList = new List<Restriction>();
            foreach (var item in command)
            {
                ValidateInput(item);
                var refItem = item;
                AssignDefaultValue(ref refItem);
                restriction = _mapper.Map<Restriction>(refItem);
                restriction.Uid = Guid.NewGuid();
                EnsureThatAllSalesAreasAdded(restriction);

                _restrictionRepository.Add(restriction);
                returnList.Add(restriction);
            }

            _restrictionRepository.SaveChanges();
            return Ok(_mapper.Map<List<RestrictionModel>>(returnList));
        }

        private void EnsureThatAllSalesAreasAdded(Restriction restriction)
        {
            if (restriction.SalesAreas != null && restriction.SalesAreas.Any() && restriction.SalesAreas.Count == _salesAreaRepository.CountAll)
            {
                restriction.SalesAreas = new List<string>();    // All sales areas added, flag as 'all'
            }
        }

        private void ValidateRestrictionDatesAndDays(DateTime startDate, DateTime? endDate, string restrictionDays)
        {
            IValidation validation = new RequiredFieldValidation
            {
                Field = new List<ValidationInfo>
                {
                    new ValidationInfo {FieldName = "Start Date", FieldToValidate = startDate},
                    new ValidationInfo {FieldName = "Restriction Days", FieldToValidate = restrictionDays}
                }
            };
            validation.Execute();

            if (endDate.HasValue)
            {
                validation = new CompareValidation
                {
                    Field = new List<ValidationInfo>
                    {
                        new ValidationInfo
                        {
                            ErrorMessage = "Restriction start date should be less than or equal to end date",
                            FieldToValidate = startDate.Date,
                            FieldToCompare = endDate?.Date,
                            Operator = Operator.LessThanEqual
                        }
                    }
                };
                validation.Execute();
            }
            const string zeroOrOne = "^(?!0{7})[0-1]{7}$";
            validation = new RegexValidation
            {
                Field = new List<ValidationInfo>
                {
                    new ValidationInfo
                    {
                        ErrorMessage = "Invalid Restriction Days",
                        FieldToValidate = restrictionDays,
                        RegexPattern = zeroOrOne
                    }
                }
            };
            validation.Execute();
        }

        private void ValidateInput(CreateRestriction command)
        {
            if (!string.IsNullOrWhiteSpace(command.ExternalIdentifier))
            {
                var restriction = _restrictionRepository.Get(command.ExternalIdentifier);
                if (restriction != null)
                {
                    var msg = $"Restriction with external identifier: '{command.ExternalIdentifier}' already exists";
                    throw new InvalidDataException(msg);
                }
            }

            ValidateRestrictionDatesAndDays(command.StartDate, command.EndDate, command.RestrictionDays);

            if (command.SalesAreas != null && command.SalesAreas.Any())
            {
                _salesAreaRepository.ValidateSaleArea(command.SalesAreas);
            }

            CustomValidation(command);

            if (!string.IsNullOrWhiteSpace(command.ProgrammeCategory) && !_programmeCategoryRepository.IsValid(
                    new List<string> { command.ProgrammeCategory },
                    out List<string> invalidList))
            {
                var msg = string.Concat("Invalid programme category: ",
                    invalidList != null ? invalidList.FirstOrDefault() : string.Empty);
                throw new InvalidDataException(msg);
            }

            if (!string.IsNullOrWhiteSpace(command.ClearanceCode))
            {
                _clearanceRepository.ValidateClearanceCode(new List<string> { command.ClearanceCode });
            }
        }

        /// <summary>
        /// Validation based on restriction type and basis
        /// </summary>
        /// <param name="command"></param>
        private static void CustomValidation(CreateRestriction command)
        {
            IValidation validation;
            switch (command.RestrictionBasis)
            {
                case RestrictionBasis.Product:
                    validation = new RequiredFieldValidation
                    {
                        Field = new List<ValidationInfo>
                        {
                            new ValidationInfo {FieldName = "Product Code", FieldToValidate = command.ProductCode}
                        }
                    };
                    validation.Execute();

                    validation = new EmptyFieldValidation
                    {
                        Field = new List<ValidationInfo>
                        {
                            new ValidationInfo
                            {
                                ErrorMessage = "Clash code must be blank/empty if basis of restriction is product",
                                FieldToValidate = command.ClashCode
                            },
                            new ValidationInfo
                            {
                                ErrorMessage = "Clearance code must be blank/empty if basis of restriction is product",
                                FieldToValidate = command.ClearanceCode
                            }
                        }
                    };
                    validation.Execute();
                    break;

                case RestrictionBasis.Clash:
                    validation = new RequiredFieldValidation
                    {
                        Field = new List<ValidationInfo>
                        {
                            new ValidationInfo {FieldName = "Clash Code", FieldToValidate = command.ClashCode}
                        }
                    };
                    validation.Execute();

                    validation = new EmptyFieldValidation
                    {
                        Field = new List<ValidationInfo>
                        {
                            new ValidationInfo
                            {
                                ErrorMessage = "Product code must be 0 if basis of restriction is clash",
                                FieldToValidate = command.ProductCode
                            },
                            new ValidationInfo
                            {
                                ErrorMessage = "Clearance code must be blank/empty if basis of restriction is clash",
                                FieldToValidate = command.ClearanceCode
                            }
                        }
                    };
                    validation.Execute();

                    if (!string.IsNullOrWhiteSpace(command.ClockNumber) &&
                        (command.ClockNumber.Length > 1 || !int.TryParse(command.ClockNumber, out var clashBasedClockNumber) || clashBasedClockNumber != 0))
                    {
                        throw new InvalidDataException("Clock number must be blank/empty if basis of restriction is clash");
                    }

                    break;

                case RestrictionBasis.ClearanceCode:
                    validation = new RequiredFieldValidation
                    {
                        Field = new List<ValidationInfo>
                        {
                            new ValidationInfo {FieldName = "Clearance Code", FieldToValidate = command.ClearanceCode}
                        }
                    };
                    validation.Execute();

                    validation = new EmptyFieldValidation
                    {
                        Field = new List<ValidationInfo>
                        {
                            new ValidationInfo
                            {
                                ErrorMessage = "Product code must be 0 if basis of restriction is clearance",
                                FieldToValidate = command.ProductCode
                            },
                            new ValidationInfo
                            {
                                ErrorMessage = "Clash code must be blank/empty if basis of restriction is clearance",
                                FieldToValidate = command.ClashCode
                            }
                        }
                    };
                    validation.Execute();

                    if (!string.IsNullOrWhiteSpace(command.ClockNumber) &&
                        (command.ClockNumber.Length > 1 || !int.TryParse(command.ClockNumber, out var clearanceBasedClockNumber) || clearanceBasedClockNumber != 0))
                    {
                        throw new InvalidDataException("Clock number must be 0 if basis of restriction is clearance");
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (command.RestrictionType)
            {
                case RestrictionType.Time:
                    validation = new EmptyFieldValidation
                    {
                        Field = new List<ValidationInfo>
                        {
                            new ValidationInfo
                            {
                                ErrorMessage =
                                    "External programme reference must be blank/empty if restriction type is time",
                                FieldToValidate = command.ExternalProgRef
                            },
                            new ValidationInfo
                            {
                                ErrorMessage = "Programme category must be blank/empty if restriction type is time",
                                FieldToValidate = command.ProgrammeCategory
                            },
                            new ValidationInfo
                            {
                                ErrorMessage =
                                    "Programme classification must be blank/empty if restriction type is time",
                                FieldToValidate = command.ProgrammeClassification
                            },
                            new ValidationInfo
                            {
                                ErrorMessage = "Index type must be 0 if restriction type is time",
                                FieldToValidate = command.IndexType
                            },
                            new ValidationInfo
                            {
                                ErrorMessage = "Index threshold must be 0 if restriction type is time",
                                FieldToValidate = command.IndexThreshold
                            }
                        }
                    };
                    validation.Execute();
                    break;

                case RestrictionType.Programme:
                    validation = new RequiredFieldValidation
                    {
                        Field = new List<ValidationInfo>
                        {
                            new ValidationInfo
                            {
                                FieldName = "External programme reference",
                                FieldToValidate = command.ExternalProgRef
                            }
                        }
                    };
                    validation.Execute();
                    validation = new EmptyFieldValidation
                    {
                        Field = new List<ValidationInfo>
                        {
                            new ValidationInfo
                            {
                                ErrorMessage =
                                    "Programme category must be blank/empty if restriction type is programme",
                                FieldToValidate = command.ProgrammeCategory
                            },
                            new ValidationInfo
                            {
                                ErrorMessage =
                                    "Programme classification must be blank/empty if restriction type is programme",
                                FieldToValidate = command.ProgrammeClassification
                            },
                            new ValidationInfo
                            {
                                ErrorMessage =
                                    "Index type must be 0 if restriction type is programme",
                                FieldToValidate = command.IndexType
                            },
                            new ValidationInfo
                            {
                                ErrorMessage =
                                    "Index threshold must be 0 if restriction type is programme",
                                FieldToValidate = command.IndexThreshold
                            }
                        }
                    };
                    validation.Execute();
                    break;

                case RestrictionType.ProgrammeCategory:
                    validation = new RequiredFieldValidation
                    {
                        Field = new List<ValidationInfo>
                        {
                            new ValidationInfo
                            {
                                FieldName = "Programme category",
                                FieldToValidate = command.ProgrammeCategory
                            }
                        }
                    };
                    validation.Execute();
                    validation = new EmptyFieldValidation
                    {
                        Field = new List<ValidationInfo>
                        {
                            new ValidationInfo
                            {
                                ErrorMessage =
                                    "External programme reference must be blank/empty if restriction type is programme category",
                                FieldToValidate = command.ExternalProgRef
                            },
                            new ValidationInfo
                            {
                                ErrorMessage =
                                    "Index type must be 0 if restriction type is programme category",
                                FieldToValidate = command.IndexType
                            },
                            new ValidationInfo
                            {
                                ErrorMessage =
                                    "Index threshold must be 0 if restriction type is programme category",
                                FieldToValidate = command.IndexThreshold
                            },
                            new ValidationInfo
                            {
                                ErrorMessage =
                                    "Programme classification must be blank/empty if restriction type is programme category",
                                FieldToValidate = command.ProgrammeClassification
                            }
                        }
                    };
                    validation.Execute();
                    break;

                case RestrictionType.Index:
                    validation = new RequiredFieldValidation
                    {
                        Field = new List<ValidationInfo>
                        {
                            new ValidationInfo {FieldName = "Index type", FieldToValidate = command.IndexType},
                            new ValidationInfo
                            {
                                FieldName = "Index threshold",
                                FieldToValidate = command.IndexThreshold
                            }
                        }
                    };
                    validation.Execute();
                    validation = new EmptyFieldValidation
                    {
                        Field = new List<ValidationInfo>
                        {
                            new ValidationInfo
                            {
                                ErrorMessage =
                                    "External programme reference must be blank/empty if restriction type is index",
                                FieldToValidate = command.ExternalProgRef
                            },
                            new ValidationInfo
                            {
                                ErrorMessage = "Programme category must be blank/empty if restriction type is index",
                                FieldToValidate = command.ProgrammeCategory
                            },
                            new ValidationInfo
                            {
                                ErrorMessage =
                                    "Programme classification must be blank/empty if restriction type is index",
                                FieldToValidate = command.ProgrammeClassification
                            }
                        }
                    };
                    validation.Execute();
                    break;

                case RestrictionType.ProgrammeClassification:
                    validation = new RequiredFieldValidation
                    {
                        Field = new List<ValidationInfo>
                        {
                            new ValidationInfo
                            {
                                FieldName = "Programme classification",
                                FieldToValidate = command.ProgrammeClassification
                            }
                        }
                    };
                    validation.Execute();
                    validation = new EmptyFieldValidation
                    {
                        Field = new List<ValidationInfo>
                        {
                            new ValidationInfo
                            {
                                ErrorMessage =
                                    "External programme reference must be blank/empty if restriction type is programme classification",
                                FieldToValidate = command.ExternalProgRef
                            },
                            new ValidationInfo
                            {
                                ErrorMessage =
                                    "Index type must be 0 if restriction type is programme classification",
                                FieldToValidate = command.IndexType
                            },
                            new ValidationInfo
                            {
                                ErrorMessage =
                                    "Index threshold must be 0 if restriction type is programme classification",
                                FieldToValidate = command.IndexThreshold
                            },
                            new ValidationInfo
                            {
                                ErrorMessage =
                                    "Programme category must be blank/empty if restriction type is programme classification",
                                FieldToValidate = command.ProgrammeCategory
                            }
                        }
                    };
                    validation.Execute();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void AssignDefaultValue(ref CreateRestriction command)
        {
            command.StartDate = command.StartDate.Date; // removing time value
            command.EndDate = command.EndDate?.Date;// removing time value
        }

        /// <summary>
        /// Deletes a set of restrictions
        /// </summary>
        /// <param name="salesAreaNames">
        /// Sales area names. If multiple sales areas are specified then it
        /// filters restrictions that are relevant for any of the sales areas as
        /// opposed to all of the sales areas
        /// </param>
        /// <param name="dateRangeStart">Date range start</param>
        /// <param name="dateRangeEnd">Date range end</param>
        /// <param name="restrictionType">Restriction type</param>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("Restrictions")]
        public IHttpActionResult Delete([FromUri] List<string> salesAreaNames = null, DateTime? dateRangeStart = null,
            DateTime? dateRangeEnd = null,
            RestrictionType? restrictionType = null)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters();
            }
            if (dateRangeStart == null && dateRangeEnd == null &&
                restrictionType == null && (salesAreaNames == null || !salesAreaNames.Any()))
            {
                _restrictionRepository.Truncate();
            }
            else
            {
                _restrictionRepository.Delete(salesAreaNames, false, dateRangeStart, dateRangeEnd, restrictionType);
            }

            return Ok();
        }

        /// <summary>
        /// Gets a set of restrictions matching the criteria, supports sorting
        /// and paging
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("Restrictions")]
        public IHttpActionResult Get([FromUri] RestrictionSearchQueryModel searchQuery)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters();
            }

            if (searchQuery == null)
            {
                searchQuery = new RestrictionSearchQueryModel();
            }

            searchQuery.DateRangeEnd = searchQuery.DateRangeEnd?.ToUniversalTime();
            searchQuery.DateRangeStart = searchQuery.DateRangeStart?.ToUniversalTime();

            var restrictions = _restrictionRepository.Get(searchQuery);

            var searchModel = new SearchResultModel<RestrictionWithDescModel>
            {
                Items = restrictions?.Items != null && restrictions.Items.Any()
                    ? _mapper.Map<List<RestrictionWithDescModel>>(restrictions.Items) : null,
                TotalCount = restrictions?.TotalCount ?? 0
            };

            return Ok(searchModel);
        }

        /// <summary>
        /// Gets a restriction
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [AuthorizeRequest("Restrictions")]
        public IHttpActionResult Get(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters();
            }
            var restriction = _restrictionRepository.GetDesc(id);
            if (restriction != null)
            {
                return Ok(_mapper.Map<RestrictionWithDescModel>(restriction));
            }
            return NotFound();
        }

        /// <summary>
        /// Deletes a restriction
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [AuthorizeRequest("Restrictions")]
        public IHttpActionResult Delete(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters();
            }
            _restrictionRepository.Delete(id);
            _restrictionRepository.SaveChanges();
            return Ok();
        }

        /// <summary>
        /// Updates a Restriction
        /// </summary>
        /// <param name="externalIdentifier"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [Route("{externalIdentifier}")]
        [AuthorizeRequest("Restrictions")]
        public IHttpActionResult Put([FromUri] string externalIdentifier,
                                     [FromBody] UpdateRestrictionModel command)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters();
            }

            if (string.IsNullOrWhiteSpace(externalIdentifier))
            {
                return this.Error().BadRequest("External identifier is required for this request");
            }

            var restriction = _restrictionRepository.Get(externalIdentifier);
            if (restriction == null)
            {
                ValidateInput(_mapper.Map<CreateRestriction>(command));

                restriction = _mapper.Map<Restriction>(command);
                restriction.Uid = Guid.NewGuid();

                EnsureThatAllSalesAreasAdded(restriction);

                restriction.ExternalIdentifier = externalIdentifier;
            }
            else
            {
                ValidateRestrictionDatesAndDays(command.StartDate, command.EndDate, command.RestrictionDays);
                UpdateRestriction(ref restriction, command);
            }

            _restrictionRepository.Add(restriction);
            _restrictionRepository.SaveChanges();

            return Ok(_mapper.Map<RestrictionModel>(restriction));
        }

        /// <summary>
        /// Updates a Restriction by internal id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [Route("internal/{id}")]
        [AuthorizeRequest("Restrictions")]
        public IHttpActionResult Put([FromUri] Guid id,
            [FromBody] UpdateRestrictionInternalModel command)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters();
            }

            var restriction = _restrictionRepository.Get(id);

            if (restriction is null)
            {
                return NotFound();
            }

            ValidateRestrictionDatesAndDays(command.StartDate, command.EndDate, command.RestrictionDays);
            UpdateRestriction(ref restriction, command);

            _restrictionRepository.Add(restriction);
            return Ok(_mapper.Map<RestrictionModel>(restriction));
        }

        private static void UpdateRestriction(ref Restriction restriction, IUpdateRestrictionCommand command)
        {
            restriction.StartDate = command.StartDate.Date;
            restriction.EndDate = command.EndDate?.Date;
            restriction.RestrictionDays = command.RestrictionDays;
            restriction.StartTime = command.StartTime;
            restriction.EndTime = command.EndTime;
            restriction.SchoolHolidayIndicator = command.SchoolHolidayIndicator;
            restriction.PublicHolidayIndicator = command.PublicHolidayIndicator;
            restriction.TimeToleranceMinsAfter = command.TimeToleranceMinsAfter;
            restriction.TimeToleranceMinsBefore = command.TimeToleranceMinsBefore;

            if (command.IndexThreshold != null)
            {
                restriction.IndexThreshold = command.IndexThreshold.Value;
            }
        }
    }
}
