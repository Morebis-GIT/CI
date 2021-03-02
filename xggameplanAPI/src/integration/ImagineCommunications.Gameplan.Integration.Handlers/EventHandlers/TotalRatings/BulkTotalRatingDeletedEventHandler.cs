using System.Linq;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.GamePlan.Domain.TotalRatings;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.TotalRatings;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.TotalRatings
{
    public class BulkTotalRatingDeletedEventHandler : IEventHandler<IBulkTotalRatingDeleted>
    {
        private readonly ITotalRatingRepository _totalRatingRepository;
        private readonly ILoggerService _logger;

        public BulkTotalRatingDeletedEventHandler(ITotalRatingRepository totalRatingRepository, ILoggerService logger)
        {
            _totalRatingRepository = totalRatingRepository;
            _logger = logger;
        }

        public void Handle(IBulkTotalRatingDeleted command)
        {
            foreach (var item in command.Data)
            {
                var totalRatings = _totalRatingRepository.Search(item.SalesArea, item.DateTimeFrom, item.DateTimeTo).ToList();

                if (totalRatings.Any())
                {
                    _totalRatingRepository.DeleteRange(totalRatings.Select(t => t.Id));
                }
                else
                {
                    _logger.Warn($"No total ratings found for sales area {item.SalesArea} and from {item.DateTimeFrom} to {item.DateTimeTo}");
                }
            }
            _totalRatingRepository.SaveChanges();
        }
    }
}
