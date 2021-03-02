using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Spots;
using xggameplan.Filters;

namespace xggameplan.Controllers
{
    [RoutePrefix("Compare")]
    public class CompareController : ApiController
    {

        private IBreakRepository _breakrepo;
        private ISpotRepository _spotrepo;
        private IProgrammeRepository _programmerepo;
        private ICampaignRepository _campaignrepo;
        private IProductRepository _productRepository;
        private IClashRepository _clashRepository;

        public CompareController(IBreakRepository breakRepository,ISpotRepository spotRepository,IProgrammeRepository programmeRepository,ICampaignRepository campaignRepository, IProductRepository productRepository, IClashRepository clashRepository)
        {
            _breakrepo = breakRepository;
            _spotrepo = spotRepository;
            _programmerepo = programmeRepository;
            _campaignrepo = campaignRepository;
            _productRepository = productRepository;
            _clashRepository = clashRepository;

        }

        /// <summary>
        /// Gets a set of record counts for all the objects we have
        /// 
        /// </summary>
        [Route("")]
        [AuthorizeRequest("Compare")]
        [ResponseType(typeof(Dictionary<string, int>))]
        public Dictionary<string, int> Get()
        {
            var breakcount = _breakrepo.GetAll().Count();
            var spotCountcount = _spotrepo.GetAll().Count(); 
            var progcount = _programmerepo.GetAll().Count(); 
            var campaigncount = _campaignrepo.GetAll().Count(); 
            var productcount = _productRepository.GetAll().Count();
            var clashcount = _clashRepository.GetAll().Count(); 

            var items = new Dictionary<string, int> {{"Breaks", breakcount},{"Spots", spotCountcount } , { "Programmes", progcount } , { "Campaigns", campaigncount } , { "Product", productcount }, { "Clash", clashcount } };

            return items;

        }
    }

}
