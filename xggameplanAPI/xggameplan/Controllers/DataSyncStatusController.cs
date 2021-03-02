using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using ImagineCommunications.BusClient.Domain.Abstractions.Repositories;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using xggameplan.Filters;
using xggameplan.model.Internal.Landmark;

namespace xggameplan.Controllers
{
    [RoutePrefix("data-sync"), AuthorizeRequest("data-sync"), FeatureFilter(nameof(ProductFeature.IntegrationSynchronization))]
    public class DataSyncStatusController : ApiController
    {
        private readonly IMapper _mapper;
        private readonly IGroupTransactionInfoRepository _infoRepository;

        public DataSyncStatusController(IMapper mapper, IGroupTransactionInfoRepository infoRepository = null)
        {
            _mapper = mapper;
            _infoRepository = infoRepository;
        }

        [HttpGet, Route("status"), ResponseType(typeof(GroupTransactionInfoModel))]
        public GroupTransactionInfoModel GetLatestGroupTransactionStatus()
        {
            var groupTransaction = _infoRepository.GetLatestExecutedGroupTransaction();

            GroupTransactionInfoModel model = groupTransaction is null
                ? null
                : _mapper.Map<GroupTransactionInfoModel>(groupTransaction);

            return model;
        }
    }
}
