using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Generic.Extensions;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using ImagineCommunications.GamePlan.Domain.Spots;

namespace xggameplan.SystemTests
{
    /// <summary>
    /// Tests schedule data (breaks, campaigns, programmes, schedules, spots etc)
    /// </summary>
    internal class ScheduleDataTest : ISystemTest
    {
        private IRepositoryFactory _repositoryFactory;
        private const string _category = "Schedule Data";

        public ScheduleDataTest(IRepositoryFactory repositoryFactory)
        {            
            _repositoryFactory = repositoryFactory;
        }

        public bool CanExecute(SystemTestCategories systemTestCategories)
        {
            return (systemTestCategories != SystemTestCategories.Deployment);     // Not relevant for installation, system can be installed before schedule data uploaded for first time
        }

        public List<SystemTestResult> Execute(SystemTestCategories systemTestCategory)
        {
            var results = new List<SystemTestResult>();

            try
            {
                using (var scope = _repositoryFactory.BeginRepositoryScope())
                {
                    var repositoryList = scope.CreateRepositories(
                        typeof(IBreakRepository),
                        typeof(ICampaignRepository),
                        typeof(IClashRepository),
                        typeof(IProductRepository),
                        typeof(IProgrammeRepository),
                        typeof(IProgrammeDictionaryRepository),
                        typeof(IRatingsScheduleRepository),
                        typeof(IScheduleRepository),
                        typeof(ISpotRepository)
                    );

                    var breakRepository = repositoryList.Get<IBreakRepository>();
                    var campaignRepository = repositoryList.Get<ICampaignRepository>();
                    var clashRepository = repositoryList.Get<IClashRepository>();
                    var productRepository = repositoryList.Get<IProductRepository>();
                    var programmeRepository = repositoryList.Get<IProgrammeRepository>();
                    var programmeDictionaryRepository = repositoryList.Get<IProgrammeDictionaryRepository>();
                    var ratingsScheduleRepository = repositoryList.Get<IRatingsScheduleRepository>();
                    var scheduleRepository = repositoryList.Get<IScheduleRepository>();
                    var spotRepository = repositoryList.Get<ISpotRepository>();

                    if (breakRepository.CountAll == 0)
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "No Break data exists. It will cause runs to fail.", ""));
                    }
                    if (campaignRepository.CountAll == 0)
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "No Campaign data exists. It will cause runs to fail.", ""));
                    }
                    if (clashRepository.CountAll == 0)
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "No Clash data exists. It will cause runs to fail.", ""));
                    }
                    if (productRepository.CountAll == 0)
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "No Product data exists. It will cause runs to fail.", ""));
                    }
                    if (programmeRepository.CountAll == 0)
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "No Programme data exists. It will cause runs to fail.", ""));
                    }
                    if (programmeDictionaryRepository.CountAll == 0)
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "No Programme Dictionary data exists. It will cause runs to fail.", ""));
                    }
                    if (ratingsScheduleRepository.CountAll == 0)
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "No Ratings Schedule data exists. It will cause runs to fail.", ""));
                    }
                    if (scheduleRepository.CountAll == 0)
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "No Schedule data exists. It will cause runs to fail.", ""));
                    }
                    if (spotRepository.CountAll == 0)
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "No Spot data exists. It will cause runs to fail.", ""));
                    }                    
                }
            }
            catch(System.Exception exception)
            {
                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Error checking schedule data: {0}", exception.Message), ""));
            }
            finally
            {
                if (!results.Where(r => r.ResultType == SystemTestResult.ResultTypes.Error).Any())
                {
                    results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Information, _category, "Schedule data test OK. Please be aware that only basic tests were performed.", ""));
                }
            }
            return results;
        }
    }
}
