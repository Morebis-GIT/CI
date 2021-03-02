using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using Raven.Abstractions.Data;
using xggameplan.core.Validators.ProductAdvertiser;
using xggameplan.Errors;
using xggameplan.Extensions;
using xggameplan.Filters;
using xggameplan.model.External;

namespace xggameplan.Controllers
{
    [RoutePrefix("ClashException")]
    public partial class ClashExceptionController : ApiController
    {
        private readonly IClashExceptionRepository _clashExceptionRepository;
        private readonly IClashRepository _clashRepository;
        private readonly IClashExceptionValidations _clashExceptionValidations;
        private readonly IProductAdvertiserValidator _productAdvertiserValidator;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ClashExceptionController(IClashExceptionRepository clashExceptionRepository,
            IClashRepository clashRepository, IProductRepository productRepository, IMapper mapper,
            IClashExceptionValidations clashExceptionValidations,
            IProductAdvertiserValidator productAdvertiserValidator)
        {
            _clashExceptionRepository = clashExceptionRepository;
            _clashRepository = clashRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _clashExceptionValidations = clashExceptionValidations;
            _productAdvertiserValidator = productAdvertiserValidator;
        }

        /// <summary>
        /// Get a set of clash exception
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("ClashException")]
        [ResponseType(typeof(SearchResultModel<ClashExceptionModel>))]
        public IHttpActionResult Get([FromUri] ClashExceptionSearchQueryModel searchQuery)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters();
            }

            // searchQuery is ref type so we can not add default value at input
            // parameter. If value not specified then return null value. so
            // adding below condition
            if (searchQuery == null)
            {
                searchQuery = new ClashExceptionSearchQueryModel();
            }

            searchQuery.EndDate = searchQuery.EndDate.ToUniversalTime();
            searchQuery.StartDate = searchQuery.StartDate.ToUniversalTime();
            var clashExceptions = _clashExceptionRepository.SearchWithDescriptions(searchQuery);
            return Ok(new SearchResultModel<ClashExceptionModel>
            {
                Items = clashExceptions?.Items != null && clashExceptions.Items.Any()
                    ? clashExceptions.Items.ToList()
                    : null,
                TotalCount = clashExceptions?.TotalCount ?? 0
            });
        }

        /// <summary>
        /// Gets a clash exception
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [AuthorizeRequest("ClashException")]
        [ResponseType(typeof(ClashExceptionModel))]
        public IHttpActionResult Get(int id)
        {
            if (id == 0)
            {
                return this.Error().InvalidParameters();
            }
            var clashException = _clashExceptionRepository.GetWithDescriptions(id);
            if (clashException == null)
            {
                return NotFound();
            }
            return Ok(clashException);
        }

        /// <summary>
        /// Save a set of clash exception
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("ClashException")]
        [ResponseType(typeof(List<ClashExceptionModel>))]
        public IHttpActionResult Post([FromBody] List<CreateClashException> command)
        {
            if (!ModelState.IsValid || command == null || !command.Any())
            {
                return this.Error().InvalidParameters();
            }
            ValidateInput(command);

            var clashExceptions = _mapper.Map<List<ClashException>>(command);
            clashExceptions.ForEach(c =>
            {
                c.StartDate = c.StartDate.Date;
                c.EndDate = (c.EndDate == null ? (DateTime?)null : c.EndDate.Value.Date);
            });

            var validationResult = ValidateForSave(clashExceptions);
            if (!validationResult.Successful)
            {
                return this.Error().BadRequest(ApiError.BadRequest(validationResult.Message));
            }

            _clashExceptionRepository.Add(clashExceptions);
            _clashExceptionRepository.SaveChanges();
            return Ok(_clashExceptionRepository.GetWithDescriptions(clashExceptions.Select(x => x.Id).ToArray()));
        }

        private void ValidateInput(List<CreateClashException> command)
        {
            command.ToList()
                .ForEach(_ => ClashException.Validation(_.StartDate, _.EndDate, _.FromValue, _.ToValue, _.TimeAndDows));

            var isAllAdvertiser = command.Where(_ => _.FromType == ClashExceptionType.Advertiser).EmptyIfNull()
                .All(_ => _.ToType == ClashExceptionType.Advertiser);
            if (!isAllAdvertiser)
            {
                throw new Exception(
                    "Clash exception to type should be advertiser if the from type is advertiser");
            }

            ValidateFromToValue(command.ToList());
        }

        private void ValidateFromToValue(List<CreateClashException> command)
        {
            var clashFromCode = command?.Where(_ => _.FromType == ClashExceptionType.Clash)
                .Select(_ => _.FromValue).Distinct(StringComparer.OrdinalIgnoreCase);
            var clashToCode = command?.Where(_ => _.ToType == ClashExceptionType.Clash)
                .Select(_ => _.ToValue).Distinct(StringComparer.OrdinalIgnoreCase);
            var clashCode = (clashFromCode ?? Enumerable.Empty<string>())
                .Union(clashToCode ?? Enumerable.Empty<string>()).ToList();
            if (clashCode.Any())
            {
                _clashRepository.ValidateClashCodes(clashCode);
            }

            var productFromCode = command?.Where(_ => _.FromType == ClashExceptionType.Product)
                .Select(_ => _.FromValue).Distinct(StringComparer.OrdinalIgnoreCase);
            var productToCode = command?.Where(_ => _.ToType == ClashExceptionType.Product)
                .Select(_ => _.ToValue).Distinct(StringComparer.OrdinalIgnoreCase);
            var productCode = (productFromCode ?? Enumerable.Empty<string>())
                .Union(productToCode ?? Enumerable.Empty<string>()).ToList();
            if (productCode.Any())
            {
                _productRepository.ValidateProductExternalIds(productCode);
            }

            var advertiserPeriodInfos = command.SelectMany(c => c.GetAdvertiserPeriodInfo()).ToArray();
            _productAdvertiserValidator.Validate(advertiserPeriodInfos);
        }

        /// <summary>
        /// Update a clash exception
        /// </summary>
        /// <param name="id"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [Route("{id}")]
        [AuthorizeRequest("ClashException")]
        [ResponseType(typeof(ClashExceptionModel))]
        public IHttpActionResult Put(int id, [FromBody] UpdateClashException command)
        {
            // MUST BE REWORKED!
            var offsetHours = 6;

            if (id == 0 || command == null)
            {
                return this.Error().InvalidParameters();
            }

            var clashException = _clashExceptionRepository.Find(id);
            if (clashException == null)
            {
                return NotFound();
            }
            ClashException.Validation(clashException.StartDate, command.EndDate, command.TimeAndDows);

            var ValidationPayload = new[]
            {
                MapToClashExceptionForValidation(command, clashException)
            };

            var validationResult = ValidateForSave(ValidationPayload);
            if (!validationResult.Successful)
            {
                return this.Error().BadRequest(ApiError.BadRequest(validationResult.Message));
            }

            MapToClashExceptions(command, ref clashException);
            _clashExceptionRepository.Add(clashException);
            _clashExceptionRepository.SaveChanges();
            return Ok(_clashExceptionRepository.GetWithDescriptions(clashException.Id));
        }

        private ClashException MapToClashExceptionForValidation(UpdateClashException command, ClashException clashException)
        {
            return new ClashException()
            {
                Id = clashException.Id,
                FromType = clashException.FromType,
                ToType = clashException.ToType,
                FromValue = clashException.FromValue,
                ToValue = clashException.ToValue,
                StartDate = clashException.StartDate,
                EndDate = command.EndDate,
                TimeAndDows = command.TimeAndDows,
                IncludeOrExclude = command.IncludeOrExclude
            };
        }

        private void MapToClashExceptions(UpdateClashException command, ref ClashException clashException)
        {
            clashException.EndDate = (command.EndDate == null ? (DateTime?)null : command.EndDate.Value.Date);
            clashException.IncludeOrExclude = command.IncludeOrExclude;
            clashException.TimeAndDows = command.TimeAndDows;
        }

        /// <summary>
        /// Delete a clash exception by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [AuthorizeRequest("ClashException")]
        [ResponseType(typeof(ClashExceptionModel))]
        public IHttpActionResult Delete(int id)
        {
            if (id == 0)
            {
                return this.Error().InvalidParameters();
            }
            _clashExceptionRepository.Remove(id);
            return Ok();
        }

        /// <summary>
        /// Delete a set of clash exception
        /// </summary>
        /// <param name="dateRangeStart"></param>
        /// <param name="dateRangeEnd"></param>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("ClashException")]
        public IHttpActionResult Delete(DateTime? dateRangeStart = null,
            DateTime? dateRangeEnd = null)
        {
            if (dateRangeStart == null && dateRangeEnd == null)
            {
                _clashExceptionRepository.Truncate();
                return Ok();
            }
            var search = new ClashExceptionSearchQueryModel()
            {
                StartDate = dateRangeStart ?? default(DateTime),
                EndDate = dateRangeEnd ?? default(DateTime)
            };
            var clashExceptions = _clashExceptionRepository.Search(search);
            if (clashExceptions?.Items == null || !clashExceptions.Items.Any())
            {
                return NotFound();
            }

            clashExceptions.Items.ToList().ForEach(_ => _clashExceptionRepository.Remove(_.Id));
            return Ok();
        }
    }
}
