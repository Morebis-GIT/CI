using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Queries;
using NodaTime;
using xggameplan.Errors;
using xggameplan.Extensions;
using xggameplan.Filters;
using xggameplan.Model;
using xggameplan.Services;
using xggameplan.Validations;

namespace xggameplan.Controllers
{
    [RoutePrefix("Product")]

    public class ProductController : ApiController
    {
        private readonly IProductRepository _repository;
        private readonly IDataChangeValidator _dataChangeValidator;
        private readonly IMapper _mapper;
        private readonly IModelDataValidator<CreateProduct> _productValidator;
        private readonly IClock _clock;

        public ProductController(
            IProductRepository repository,
            IDataChangeValidator dataChangeValidator,
            IMapper mapper,
            IModelDataValidator<CreateProduct> productValidator,
            IClock clock)
        {
            _repository = repository;
            _dataChangeValidator = dataChangeValidator;
            _mapper = mapper;
            _productValidator = productValidator;
            _clock = clock;
        }

        /// <summary>
        /// Get all Products
        /// </summary>
        [Route("")]
        [AuthorizeRequest("Product")]
        [ResponseType(typeof(IEnumerable<ProductModel>))]
        public IEnumerable<ProductModel> Get()
        {
            var products = _repository.GetAll();
            if (products != null && products.Any())
            {
                return _mapper.Map<List<ProductModel>>(products.ToList());
            }

            return null;
        }

        /// <summary>
        /// Gets a Product
        /// </summary>
        /// <param name="id"></param>
        [Route("{id}")]
        [AuthorizeRequest("Product")]
        [ResponseType(typeof(ProductModel))]
        public IHttpActionResult Get(Guid id)
        {
            var item = _repository.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<ProductModel>(item));
        }

        /// <summary>
        /// Gets Product by externalId
        /// </summary>
        /// <param name="externalId"></param>
        [Route("externalref/{externalId}")]
        [AuthorizeRequest("Product")]
        [ResponseType(typeof(ProductModel))]
        public IHttpActionResult Get(string externalId)
        {
            var item = _repository.FindByExternal(externalId).FirstOrDefault();
            return Ok(_mapper.Map<ProductModel>(item));
        }

        /// <summary>
        /// Search the Advertisers
        /// </summary>
        /// <param name="queryModel"></param>
        [Route("Advertiser/Search")]
        [AuthorizeRequest("Product")]
        [ResponseType(typeof(SearchResultModel<ProductAdvertiserModel>))]
        public IHttpActionResult GetAdvertiser([FromUri] AdvertiserSearchQueryModel queryModel)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters("One or more of the required query parameters are missing.");
            }
            if (queryModel == null)
            {
                queryModel = new AdvertiserSearchQueryModel();
            }

            var products = _repository.Search(queryModel, _clock.GetCurrentInstant().ToDateTimeUtc());

            var searchModel = new SearchResultModel<ProductAdvertiserModel>()
            {
                Items = products.Items.ToList(),

                TotalCount = products.TotalCount
            };

            return Ok(searchModel);
        }

        /// <summary>
        /// Search the Products
        /// </summary>
        /// <param name="queryModel"></param>
        [Route("Search")]
        [AuthorizeRequest("Product")]
        [ResponseType(typeof(SearchResultModel<ProductNameModel>))]
        public IHttpActionResult Get([FromUri] ProductSearchQueryModel queryModel)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters("One or more of the required query parameters are missing.");
            }
            if (queryModel == null)
            {
                queryModel = new ProductSearchQueryModel();
            }
            var products = _repository.Search(queryModel, _clock.GetCurrentInstant().ToDateTimeUtc());

            var searchModel = new SearchResultModel<ProductNameModel>()
            {
                Items = products.Items.ToList(),
                TotalCount = products.TotalCount
            };

            return Ok(searchModel);
        }

        /// <summary>
        /// Creates a set of Products
        /// </summary>
        [Route("")]
        [AuthorizeRequest("Product")]
        public IHttpActionResult Post([FromBody] IEnumerable<CreateProduct> products)
        {
            if (products == null || !ModelState.IsValid)
            {
                return InternalServerError();
            }

            var items = _mapper.Map<List<Product>>(products);

            foreach (var b in items)
            {
                b.Uid = Guid.NewGuid();
            }
            _repository.Add(items);
            return Ok();
        }

        /// <summary>
        /// Update or create Product by externalId
        /// </summary>
        /// <param name="externalId"></param>
        /// <param name="command"></param>
        [Route("externalref/{externalId}")]
        [AuthorizeRequest("Product")]
        [ResponseType(typeof(ProductModel))]
        public IHttpActionResult Put([FromUri] string externalId, [FromBody] CreateProduct command)
        {
            if (command != null && externalId != command.Externalidentifier)
            {
                ModelState.AddModelError(nameof(CreateProduct.Externalidentifier), "External Id does not match");
                return BadRequest(ModelState);
            }

            if (!_productValidator.IsValid(command))
            {
                return _productValidator.BadRequest();
            }

            var newProduct = _mapper.Map<Product>(command);
            var productToUpdate = _repository.FindByExternal(externalId).FirstOrDefault();

            if (productToUpdate == null)
            {
                productToUpdate = newProduct;
                productToUpdate.Uid = Guid.NewGuid();
                _repository.Add(productToUpdate);
            }
            else
            {
                productToUpdate.Update(newProduct);
                _repository.Update(productToUpdate);
            }

            _repository.SaveChanges(); // Do not remove this, need to persist changes now so that we can return ProductModel

            return Ok(_mapper.Map<ProductModel>(productToUpdate));
        }

        /// <summary>
        /// Deletes a single Product
        /// </summary>
        /// <param name="id"></param>
        [Route("{id}")]
        [AuthorizeRequest("Product")]
        public IHttpActionResult Delete(Guid id)
        {
            _repository.Remove(id);
            return Ok();
        }

        /// <summary>
        /// Delete all Products
        /// </summary>
        [Route("DeleteAll")]
        [AuthorizeRequest("Product")]
        public async Task<IHttpActionResult> DeleteAsync()
        {
            // Validate that we can delete
            _dataChangeValidator.ThrowExceptionIfAnyErrors(
                _dataChangeValidator.ValidateChange<Product>(
                    ChangeActions.Delete,
                    ChangeTargets.AllItems,
                    null
                    ));

            await _repository.TruncateAsync();

            return Ok();
        }
    }
}
