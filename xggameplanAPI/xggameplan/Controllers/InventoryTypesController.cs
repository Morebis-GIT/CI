using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Repositories;
using xggameplan.Filters;
using xggameplan.model.External;

namespace xggameplan.Controllers
{
    /// <summary>
    /// API for inventory types.
    /// </summary>
    [RoutePrefix("inventory-types")]
    [FeatureFilter(nameof(ProductFeature.InventoryStatus))]
    public class InventoryTypesController : ApiController
    {
        private static readonly string[] ExcludedInventoryTypeCodes = { "LU", "OP" };

        private readonly IInventoryTypeRepository _inventoryTypeRepository;
        private readonly IMapper _mapper;

        public InventoryTypesController(IInventoryTypeRepository inventoryTypeRepository, IMapper mapper)
        {
            _inventoryTypeRepository = inventoryTypeRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the list of available inventory types.
        /// </summary>
        /// <returns>The list of available inventory types.</returns>
        [Route("")]
        [AuthorizeRequest("inventory-types")]
        [ResponseType(typeof(List<InventoryTypeModel>))]
        public IHttpActionResult GetInventoryTypes()
        {
            var inventoryTypes = _inventoryTypeRepository
                .GetSystemInventories()
                .Where(t => !ExcludedInventoryTypeCodes.Contains(t.InventoryCode.Trim()))
                .ToList();
            var result = _mapper.Map<List<InventoryTypeModel>>(inventoryTypes);

            return Ok(result);
        }
    }
}
