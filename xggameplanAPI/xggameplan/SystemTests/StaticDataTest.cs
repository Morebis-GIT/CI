using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Settings;
using ImagineCommunications.GamePlan.Domain.Generic.Extensions;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings;
using ImagineCommunications.GamePlan.Domain.OutputFiles;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Languages;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;
using ImagineCommunications.GamePlan.Domain.Shared.ProgrammeClassifications;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.System.AccessTokens;
using ImagineCommunications.GamePlan.Domain.Shared.System.Users;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using ImagineCommunications.GamePlan.Domain.SmoothFailureMessages;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Microsoft.Extensions.Configuration;
using Raven.Client;
using xggameplan.AuditEvents;

namespace xggameplan.SystemTests
{
    /// <summary>
    /// Tests static data
    /// </summary>
    internal class StaticDataTest : ISystemTest
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IConfiguration _applicationConfiguration;
        private const string _category = "Static Data";

        public StaticDataTest(IRepositoryFactory repositoryFactory, IConfiguration applicationConfiguration)
        {
            _repositoryFactory = repositoryFactory;
            _applicationConfiguration = applicationConfiguration;
        }

        public bool CanExecute(SystemTestCategories systemTestCategories) => true;

        private List<SystemTestResult> TestInternalUsers()
        {
            var results = new List<SystemTestResult>();

            string connectionString = _applicationConfiguration["db:Master:connectionString"];
            using (IDocumentStore documentStore = DocumentStoreFactory.CreateStore(connectionString, System.Reflection.Assembly.GetExecutingAssembly()))
            {
                using (IDocumentSession session = documentStore.OpenSession())
                {
                    // Set internal user access tokens
                    List<Guid> internalUserAccessTokens = new List<Guid>()
                    {
                        new Guid("74fe2f3c-2d2c-4fa4-879a-4984f18fe979"),   // Tasks
                        new Guid("8df22a0d-bbc3-4ab5-9359-b6f235e873cd"),   // Frontend
                        new Guid("77648dbd-e6de-49df-9d75-c4513ab41029")    // AutoBook
                    };

                    foreach (var internalUserAccessToken in internalUserAccessTokens)
                    {
                        var token = session.GetAll<AccessToken>(x => x.Token == internalUserAccessToken.ToString()).Find(x => x.Token == internalUserAccessToken.ToString());
                        if (token == null)
                        {
                            results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, String.Format("Unable to find access token {0} for internal users. Internal applications will be unable to use Gameplan API.", internalUserAccessToken), ""));
                        }
                        else
                        {
                            // Check that the user exists
                            var user = session.Query<User>().Where(u => u.Id == token.UserId).FirstOrDefault();
                            if (user == null)
                            {
                                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, String.Format("Unable to find the internal user for access token {0}. Internal applications will be unable to use Gameplan API.", internalUserAccessToken), ""));
                            }
                        }
                    }
                }
            }

            if (!results.Any())
            {
                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Information, _category, "Internal users test OK", ""));
            }
            return results;
        }

        public List<SystemTestResult> Execute(SystemTestCategories systemTestCategory)
        {
            var results = new List<SystemTestResult>();

            try
            {
                using (var scope = _repositoryFactory.BeginRepositoryScope())
                {
                    var repositoryList = scope.CreateRepositories(
                        typeof(IFunctionalAreaRepository),
                        typeof(IISRSettingsRepository),
                        typeof(IRSSettingsRepository),
                        typeof(ILanguageRepository),
                        typeof(IMetadataRepository),
                        typeof(ISalesAreaRepository),
                        typeof(IOutputFileRepository),
                        typeof(IUniverseRepository),
                        typeof(IEmailAuditEventSettingsRepository),
                        typeof(IAutoBookInstanceConfigurationRepository),
                        typeof(IAutoBookSettingsRepository),
                        typeof(ISmoothFailureMessageRepository),
                        typeof(IDemographicRepository),
                        typeof(IProgrammeClassificationRepository),
                        typeof(IProgrammeCategoryHierarchyRepository));

                    var functionalAreaRepository = repositoryList.Get<IFunctionalAreaRepository>();
                    var languageRepository = repositoryList.Get<ILanguageRepository>();
                    var metadataRepository = repositoryList.Get<IMetadataRepository>();
                    var salesAreaRepository = repositoryList.Get<ISalesAreaRepository>();
                    var outputFileRepository = repositoryList.Get<IOutputFileRepository>();
                    var isrSettingsRepository = repositoryList.Get<IISRSettingsRepository>();
                    var rsSettingsRepository = repositoryList.Get<IRSSettingsRepository>();
                    var universeRepository = repositoryList.Get<IUniverseRepository>();
                    var autoBookInstanceConfigurationRepository = repositoryList.Get<IAutoBookInstanceConfigurationRepository>();
                    var autoBookSettingsRepository = repositoryList.Get<IAutoBookSettingsRepository>();
                    var smoothFailureMessageRepository = repositoryList.Get<ISmoothFailureMessageRepository>();
                    var demographicRepository = repositoryList.Get<IDemographicRepository>();
                    var programmeClassificationRepository = repositoryList.Get<IProgrammeClassificationRepository>();
                    var programmeCategoryRepository = repositoryList.Get<IProgrammeCategoryHierarchyRepository>();

                    var salesAreas = salesAreaRepository.GetAll().ToList();

                    /*
                    if (demographicRepository.GetAll().ToList().Count == 0)
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, "No Demographic data exists. It will cause runs to fail."));
                    }
                    */
                    if (demographicRepository.CountAll == 0)
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "No Demographic data exists. It will cause runs to fail.", ""));
                    }
                    if (metadataRepository.GetByKey(MetaDataKeys.BreakTypes).Count == 0)
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "No Break Types data exists. It will cause runs to fail.", ""));
                    }
                    if (functionalAreaRepository.GetAll().ToList().Count == 0)
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "No Functional Area data exists. It will cause errors displaying Failure data when the run is complete.", ""));
                    }
                    if (languageRepository.GetAll().ToList().Count == 0)
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "No Language data exists.", ""));
                    }
                    if (salesAreaRepository.GetAll().ToList().Count == 0)
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "No Sales Area data exists. It will cause runs to fail.", ""));
                    }
                    if (outputFileRepository.GetAll().ToList().Count == 0)
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "No Output File data exists. It will cause runs to fail. It is needed for processing output files.", ""));
                    }

                    var isrSettingsList = isrSettingsRepository.GetAll();
                    if (isrSettingsList.Count == 0)
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "No ISR Settings data exists. It will cause runs to fail if ISR is requested.", ""));
                    }
                    else if (isrSettingsList.Count < salesAreas.Count)
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "ISR Settings data is missing for some sales areas. It will cause runs to fail if ISR is requested.", ""));
                    }

                    var rsSettingsList = rsSettingsRepository.GetAll();
                    if (rsSettingsList.Count == 0)
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "No Right Sizer Settings data exists. It will cause runs to fail if Right Sizer is requested.", ""));
                    }
                    else if (rsSettingsList.Count < salesAreas.Count)
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "Right Sizer Settings data is missing for some sales areas. It will cause runs to fail if Right Sizer is requested.", ""));
                    }

                    if (universeRepository.GetAll().ToList().Count == 0)
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "No Universe data exists. It will cause runs to fail.", ""));
                    }

                    if (programmeClassificationRepository.CountAll == 0)
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "No Programme Classificiation data exists. It will cause runs to fail.", ""));        // Not mandatory
                    }

                    if (programmeCategoryRepository.GetAll().ToList().Count == 0)
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "No Programme Category Hierarchy data exists. It will cause runs to fail.", ""));       
                    }

                    // Test internal users
                    //results.AddRange(TestInternalUsers());                    
                }
            }
            catch (System.Exception exception)
            {
                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, String.Format("Error checking static data: {0}", exception.Message), ""));
            }
            finally
            {
                if (!results.Where(r => r.ResultType == SystemTestResult.ResultTypes.Error).Any())
                {
                    results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Information, _category, "Static data test OK", ""));
                }
            }
            return results;
        }
    }
}
