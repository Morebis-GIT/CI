using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.OutputFiles;
using ImagineCommunications.GamePlan.Domain.OutputFiles.Objects;
using ImagineCommunications.GamePlan.Domain.Recommendations;
using ImagineCommunications.GamePlan.Domain.ResultsFiles;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignFailures;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignFailures.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults;
using ImagineCommunications.GamePlan.Domain.ScenarioFailures;
using ImagineCommunications.GamePlan.Domain.ScenarioFailures.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using Newtonsoft.Json;
using xggameplan.AutoBooks.AWS;
using xggameplan.cloudaccess.Factory;
using xggameplan.common.Model;
using xggameplan.core.Interfaces;
using xggameplan.core.ReportGenerators;
using xggameplan.core.ReportGenerators.Interfaces;
using xggameplan.core.ReportGenerators.ScenarioCampaignFailures;
using xggameplan.core.ReportGenerators.ScenarioCampaignResults;
using xggameplan.Errors;
using xggameplan.Extensions;
using xggameplan.KPIProcessing.Abstractions;
using xggameplan.Model;
using xggameplan.OutputFiles;
using xggameplan.OutputFiles.Filtering.SQL;
using xggameplan.Services;

namespace xggameplan.Controllers
{
    /// <summary>
    /// The Scenario Results controller.
    /// </summary>
    /// <remarks>
    /// Some of the endpoints currently have 2 routes in an effort to standardise route formats.
    /// </remarks>
    /// <seealso cref="RecommendationsController">
    /// All endpoints for extended recommendations have been moved to the Recommendations Controller.
    /// </seealso>
    [RoutePrefix("ScenarioResults")]
    public partial class ScenarioResultsController : ApiController
    {
        private const string RunNotFoundMessage = "Run was not found";

        private readonly IRunRepository _runRepository;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly ISalesAreaRepository _salesAreaRepository;
        private readonly IScenarioResultRepository _scenarioResultRepository;
        private readonly IOutputFileRepository _outputFileRepository;
        private readonly IResultsFileRepository _resultsFileRepository;
        private readonly IFailuresRepository _failuresRepository;
        private readonly IRecommendationRepository _recommendationRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IProductRepository _productRepository;
        private readonly IClashRepository _clashRepository;
        private readonly IScenarioRepository _scenarioRepository;
        private readonly IScenarioCampaignResultRepository _scenarioCampaignResultRepository;
        private readonly IMapper _mapper;
        private readonly IScenarioCampaignFailureRepository _scenarioCampaignFailureRepository;
        private readonly IFunctionalAreaRepository _functionalAreaRepository;
        private readonly ITenantSettingsRepository _tenantSettingsRepository;
        private readonly IFactory _storageClientFactory;
        private readonly AWSSettings _awsSettings;
        private readonly IRecommendationAggregator _recommendationAggregator;
        private readonly IScenarioCampaignResultReportCreator _scenarioCampaignResultReportCreator;
        private readonly IKPICalculationScopeFactory _kpiCalculationScopeFactory;

        public ScenarioResultsController(ISalesAreaRepository salesAreaRepository, IRunRepository runRepository, IFailuresRepository failuresRepository,
            IScenarioResultRepository scenarioResultRepository, IResultsFileRepository resultsFileRepository, IOutputFileRepository outputFileRepository,
            IRecommendationRepository recommendationRepository, ICampaignRepository campaignRepository, IProductRepository productRepository, IMapper mapper,
            IClashRepository clashRepository, IScenarioRepository scenarioRepository, IScenarioCampaignResultRepository scenarioCampaignResultRepository,
            IScenarioCampaignFailureRepository scenarioCampaignFailureRepository, IFunctionalAreaRepository functionalAreaRepository,
            ITenantSettingsRepository tenantSettingsRepository, IFactory storageClientFactory, AWSSettings awsSettings,
            IRecommendationAggregator recommendationAggregator, IScenarioCampaignResultReportCreator scenarioCampaignResultReportCreator,
            IKPICalculationScopeFactory kpiCalculationScopeFactory)
        {
            _salesAreaRepository = salesAreaRepository;
            _runRepository = runRepository;
            _scenarioResultRepository = scenarioResultRepository;
            _resultsFileRepository = resultsFileRepository;
            _outputFileRepository = outputFileRepository;
            _failuresRepository = failuresRepository;
            _recommendationRepository = recommendationRepository;
            _campaignRepository = campaignRepository;
            _productRepository = productRepository;
            _clashRepository = clashRepository;
            _scenarioRepository = scenarioRepository;
            _scenarioCampaignResultRepository = scenarioCampaignResultRepository;
            _mapper = mapper;
            _tenantSettingsRepository = tenantSettingsRepository;
            _scenarioCampaignFailureRepository = scenarioCampaignFailureRepository;
            _functionalAreaRepository = functionalAreaRepository;
            _storageClientFactory = storageClientFactory;
            _awsSettings = awsSettings;
            _recommendationAggregator = recommendationAggregator;
            _scenarioCampaignResultReportCreator = scenarioCampaignResultReportCreator;
            _kpiCalculationScopeFactory = kpiCalculationScopeFactory;
        }

        /// <summary>
        /// Returns scenario result by scenarioId
        ///
        ///
        ///
        /// NOTE: Failures will eventually be removed from the return value.
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{scenarioId}")]
        [ResponseType(typeof(ScenarioResultModel))]
        public IHttpActionResult Get(Guid scenarioId)
        {
            var scenarioResult = _scenarioResultRepository.Find(scenarioId);
            if (scenarioResult == null)
            {
                return NotFound();
            }
            var scenarioResultModel = _mapper.Map<ScenarioResultModel>(scenarioResult);

            // Add failures. This is a temporary solution until the frontend is
            // changed. Originally then failures were stored in the ScenarioResult document.
            // TODO: Remove Failures property from ScenarioResultModel when frontend has been changed.
            var failures = _failuresRepository.Get(scenarioId);
            if (failures != null)
            {
                var failuresList = GetFailureModels(failures.Items);
                failuresList.ForEach(item => scenarioResultModel.Failures.Add(item));
            }

            Run run = _runRepository.FindByScenarioId(scenarioId);
            scenarioResultModel.Run = new RunReference()
            {
                Id = run.Id,
                Description = run.Description
            };
            return Ok(scenarioResultModel);
        }

        /// <summary>
        /// Returns CSV from SQL query run against output files
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{scenarioId}/outputfiles/query/sql")]
        public IHttpActionResult Post([FromUri] Guid scenarioId,
                                      [FromBody] OutputFileSQLFilterModel command)
        {
            if (command == null || !ModelState.IsValid)
            {
                return this.Error().InvalidParameters("Invalid parameters");
            }

            ScenarioResult scenarioResult = _scenarioResultRepository.Find(scenarioId);
            if (scenarioResult == null)
            {
                return NotFound();
            }

            if (String.IsNullOrEmpty(command.SQL))
            {
                return this.Error().InvalidParameters("SQL is invalid");
            }

            // Get temp data folder
            string tempFolder = System.Web.Hosting.HostingEnvironment.MapPath("/Temp");
            string outputFilesFolder = string.Format(@"{0}\OutputFiles", tempFolder);

            SQLOutputFileFilterSettings filterSettings = null;

            string compressedResultsFile = "";

            LocalOutputFiles localOutputFiles = new LocalOutputFiles(_resultsFileRepository, outputFilesFolder);

            try
            {
                // Set filter settings,
                filterSettings = new SQLOutputFileFilterSettings()
                {
                    OutputFileFolder = string.Format(@"{0}\{1}\Data", outputFilesFolder, scenarioId),     // Same folder that LocalOutputFileStorage stores files
                    SQL = command.SQL,      // "select * from lmkii_spot_reqm.txt",
                    ResultsFile = string.Format(@"{0}\{1}\Results-{2}.txt", outputFilesFolder, scenarioId, Guid.NewGuid()),  // Make unique in case two instances are querying
                    Delimiter = ','
                };

                // Download all output files referenced by query
                localOutputFiles.GetOutputFiles(scenarioId, false, GetOutputFilesForSQL(filterSettings.SQL));

                // Apply filter
                SQLOutputFileFilter filter = new SQLOutputFileFilter();
                filter.Filter(filterSettings);

                /*
                var result = new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = new System.Net.Http.ByteArrayContent(System.IO.File.ReadAllBytes(filterSettings.OutputFile)),
                };
                */
                // Return results
                /*
                System.IO.Stream compressedStream = CompressionUtilities.CompressGZip(System.IO.File.OpenRead(filterSettings.OutputFile));
                var result = new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = new System.Net.Http.StreamContent(compressedStream)
                };

                result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = string.Format("Results-{0}.zip", id),
                };
                */

                // Return file
                compressedResultsFile = System.IO.Path.Combine(outputFilesFolder, string.Format("{0}.zip", Guid.NewGuid()));
                CompressionUtilities.CompressGZipToFile(filterSettings.ResultsFile, compressedResultsFile);
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(System.IO.File.ReadAllBytes(compressedResultsFile))
                };
                result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                return new System.Web.Http.Results.ResponseMessageResult(result);
            }
            finally
            {
                // Clean up
                localOutputFiles.DeleteOutputFiles(scenarioId);

                // Delete uncompressed results file
                if (!String.IsNullOrEmpty(filterSettings?.ResultsFile) && System.IO.File.Exists(filterSettings.ResultsFile))
                {
                    System.IO.File.Delete(filterSettings.ResultsFile);
                }

                // Delete compressed results file
                if (!String.IsNullOrEmpty(compressedResultsFile) && System.IO.File.Exists(compressedResultsFile))
                {
                    System.IO.File.Delete(compressedResultsFile);
                }
            }
        }

        /// <summary>
        /// Returns all output files referenced by the SQL.
        ///
        /// It is possible that this function might return files that it
        /// shouldn't. It just means an un-necessary Raven file download
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private List<OutputFile> GetOutputFilesForSQL(string sql)
        {
            List<OutputFile> outputFilesFound = new List<OutputFile>();
            foreach (OutputFile outputFile in _outputFileRepository.GetAll())
            {
                if (sql.ToUpper().Contains(outputFile.QueryFileName.ToUpper()))
                {
                    outputFilesFound.Add(outputFile);
                }
            }
            return outputFilesFound;
        }

        /// <summary>
        /// Returns output file for scenario result by scenarioId
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{scenarioId}/outputfiles/{fileId}")]
        public IHttpActionResult Get5([FromUri] Guid scenarioId,
                                      [FromUri] string fileId)
        {
            // Check output file is valid
            if (!fileId.Contains("."))      // Stupid hack, route doesn't work if contains full stop
            {
                fileId += ".out";
            }
            OutputFile outputFile = _outputFileRepository.Find(fileId);
            if (outputFile == null)
            {
                return NotFound();
            }

            // Get temp data folder
            string tempFolder = System.Web.Hosting.HostingEnvironment.MapPath("/Temp");
            string outputFilesFolder = Path.Combine(tempFolder, "OutputFiles");

            LocalOutputFiles localOutputFiles = new LocalOutputFiles(_resultsFileRepository, outputFilesFolder);

            try
            {
                // Get output files in compressed format
                localOutputFiles.GetOutputFiles(scenarioId, true, new List<OutputFile>() { outputFile });

                // Returns file
                string outputFilePath = localOutputFiles.GetOutputFilePath(scenarioId, true, outputFile);
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(File.ReadAllBytes(outputFilePath))
                };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                return new System.Web.Http.Results.ResponseMessageResult(result);
            }
            finally
            {
                // Delete downloaded output files
                localOutputFiles.DeleteOutputFiles(scenarioId);
            }
        }

        /// <summary>
        /// Deletes scenario result
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{scenarioId}")]
        public IHttpActionResult Delete(Guid scenarioId)
        {
            var scenarioResult = _scenarioResultRepository.Find(scenarioId);
            if (scenarioResult == null)
            {
                return NotFound();
            }
            else
            {
                _scenarioResultRepository.Remove(scenarioId);
            }
            return this.NoContent();
        }

        /// <summary>
        /// Gets recommendations for scenario. Smooth recommendations are stored
        /// against the first ScenarioId for the run
        /// </summary>
        /// <param name="id"></param>
        [HttpGet]
        [Route("{id}/recommendations")]
        [ResponseType(typeof(IEnumerable<RecommendationModel>))]
        public IHttpActionResult GetRecommendations(Guid id)
        {
            // Allow find by ScenarioId (original) or RunId
            var run = _runRepository.Find(id);
            if (run == null)        // Not a RunId passed, check if a ScenarioId
            {
                run = _runRepository.FindByScenarioId(id);
            }
            if (run == null)
            {
                return NotFound();
            }
            bool isScenarioId = (run.Id != id);
            var recommendations = _recommendationRepository.GetByScenarioId(isScenarioId ? id : run.Scenarios[0].Id);
            var items = _mapper.Map<List<RecommendationModel>>(recommendations);

            // For Smooth recommendations only then return "Unplaced", instead
            // of null, for unplaced breaks
            foreach (var item in items)
            {
                if (item.Processor == "smooth" && String.IsNullOrEmpty(item.ExternalBreakNo))
                {
                    item.ExternalBreakNo = Globals.UnplacedBreakString;
                }
            }
            return Ok(items);
        }

        /// <summary>
        /// Gets reduced recommendations for scenario
        /// </summary>
        /// <param name="id"></param>
        /// <param name="externalCampaignNumbers">
        /// Recommendations are searched for with these external campaign numbers.
        /// </param>
        /// <param name="salesAreaNames">
        /// Recommendations are searched for with these sales area names.
        /// </param>
        /// <returns>Returns a URL</returns>
        [HttpGet]
        [Route("{id}/recommendations/basic/export/get-link")]
        [Route("{id}/recommendations/basic/export/link")]
        public IHttpActionResult GetReducedRecommendations(
            [FromUri] Guid id,
            [FromUri] List<string> externalCampaignNumbers = null,
            [FromUri] List<string> salesAreaNames = null
            )
        {
            var run = _runRepository.Find(id);

            if (run is null)
            {
                run = _runRepository.FindByScenarioId(id);
            }

            if (run is null)
            {
                return Content(HttpStatusCode.NotFound, "Invalid run ID or scenario ID");
            }

            bool isScenarioId = run.Id != id;
            var scenarioId = isScenarioId ? id : run.Scenarios[0].Id;

            var descriptor = new S3FileComment()
            {
                BucketName = _awsSettings.S3Bucket,
                FileNameWithPath = $"reports/ReducedRecommendations-{scenarioId.ToString()}-{DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")}.json"
            };

            var engine = _storageClientFactory.GetEngine();

            try
            {
                if (!engine.FileExists(descriptor))
                {
                    var dataSnapshot = GenerateReducedRecommendationsResultsDataShapshot(scenarioId, externalCampaignNumbers, salesAreaNames);
                    if (dataSnapshot.data != null)
                    {
                        var jsonStr = JsonConvert.SerializeObject(dataSnapshot.data);
                        using (Stream mStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonStr)))
                        {
                            engine.Upload(new S3UploadComment()
                            {
                                BucketName = descriptor.BucketName,
                                DestinationFilePath = descriptor.FileNameWithPath,
                                FileStream = mStream
                            });
                        }
                    }
                    else
                    {
                        return Content(HttpStatusCode.NotFound, "No recommendations found");
                    }
                }
            }
            catch
            {
                return Content(HttpStatusCode.InternalServerError, "Error creating file");
            }

            return Content(HttpStatusCode.OK, engine.GetPreSignedUrl(descriptor));
        }

        /// <summary>
        /// Gets top failures for scenario.
        /// </summary>
        /// <param name="scenarioId"></param>
        [HttpGet]
        [Route("{scenarioId}/topfailures")]
        [ResponseType(typeof(Dictionary<int, int>))]
        public IHttpActionResult GetTopFailures(Guid scenarioId)
        {
            var scenario = _failuresRepository.Get(scenarioId);

            if (scenario == null)
            {
                return NotFound();
            }

            var vvList = scenario.Items.GroupBy(x => x.Type).Select(g => new { g.Key, Count = g.Sum(_ => _.Failures) })
                .OrderByDescending(g => g.Count);

            return Ok(vvList);
        }

        /// <summary>
        /// Get unique failure types for a scenario and its siblings
        /// </summary>
        /// <param name="scenarioId"></param>
        [HttpGet]
        [Route("{scenarioId}/failuretypes")]
        [ResponseType(typeof(List<int>))]
        public IHttpActionResult GetUniqueFailureTypesForScenarioAndItsSiblings(Guid scenarioId)
        {
            var run = _runRepository.GetByScenarioId(scenarioId);

            if (run is null || run.Count() != 1)
            {
                return NotFound();
            }

            var scenarioIds = new List<Guid>();
            run.First().Scenarios.ForEach(s => scenarioIds.Add(s.Id));

            return Ok(GetFailureTypes(scenarioIds));
        }

        /// <summary>
        /// Gets metrics for scenario.
        /// </summary>
        /// <param name="scenarioId"></param>
        [HttpGet]
        [Route("{scenarioId}/metrics")]
        [ResponseType(typeof(IEnumerable<ScenarioResult>))]
        public IHttpActionResult GetMetrics(Guid scenarioId)
        {
            var scenario = _scenarioResultRepository.Find(scenarioId);

            if (scenario == null)
            {
                return NotFound();
            }

            return Ok(scenario);
        }

        /// <summary>
        /// Gets Group results for scenario.
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <param name="groupbynames"></param>
        [HttpGet]
        [Route("{scenarioId}/drilldown")]
        [ResponseType(typeof(GroupResult))]
        public IHttpActionResult GetDrilldown(Guid scenarioId, [FromUri] string[] groupbynames)
        {
            var recommendations = _recommendationRepository.GetByScenarioId(scenarioId);

            if (recommendations == null)
            {
                return NotFound();
            }

            if (groupbynames == null)
            {
                return BadRequest();
            }

            var groupByMany = recommendations.GroupByMany("SpotEfficiency", groupbynames);

            return Ok(groupByMany);
        }

        [HttpGet]
        [Route("{scenarioId}/campaignresults/export/get-link")]
        [Route("{scenarioId}/campaignresults/export/link")]
        public IHttpActionResult GetExportScenarioCampaignResultsLink([FromUri] Guid scenarioId)
        {
            var engine = _storageClientFactory.GetEngine();

            var run = _runRepository.FindByScenarioId(scenarioId);
            if (run is null)
            {
                return this.Error().ResourceNotFound(RunNotFoundMessage);
            }

            var scenario = _scenarioRepository.Get(scenarioId);
            var filePath = ReportFileNameHelper.ScenarioCampaignResult(scenario, run.ExecuteStartedDateTime.Value);

            var descriptor = new S3FileComment()
            {
                BucketName = _awsSettings.S3Bucket,
                FileNameWithPath = filePath
            };

            if (!engine.FileExists(descriptor))
            {
                using (var excelFileStream =
                    _scenarioCampaignResultReportCreator.GenerateReport(
                        ScenarioCampaignResultReportHelper.MapToExtendedResults(
                            _scenarioCampaignResultRepository.Get(scenarioId)?.Items, scenario.CampaignPassPriorities, _mapper)))
                {
                    engine.Upload(new S3UploadComment()
                    {
                        BucketName = descriptor.BucketName,
                        DestinationFilePath = descriptor.FileNameWithPath,
                        FileStream = excelFileStream
                    });
                }
            }

            return Content(HttpStatusCode.OK, engine.GetPreSignedUrl(descriptor));
        }

        /// <summary>
        /// Get scenario's campaign results by scenario id.
        /// </summary>
        /// <param name="scenarioId"></param>
        [HttpGet]
        [Route("{scenarioId}/campaignresults")]
        [ResponseType(typeof(List<ScenarioCampaignResultModel>))]
        public IHttpActionResult GetScenarioCampaignResults([FromUri] Guid scenarioId)
        {
            var reportData = _scenarioCampaignResultRepository.Get(scenarioId);

            if (reportData is null)
            {
                return NotFound();
            }

            return Ok(_scenarioCampaignResultReportCreator.GenerateReportData(
                ScenarioCampaignResultReportHelper.MapToExtendedResults(reportData.Items, _scenarioRepository.Get(scenarioId).CampaignPassPriorities, _mapper)));
        }

        /// <summary>
        /// Gets simple recommendations for scenario. Only Optimiser, ISR and
        /// Right Sizer recommendations are returned.
        /// </summary>
        /// <param name="scenarioId"></param>
        [HttpGet]
        [Route("{scenarioId}/recommendations/simple")]
        [ResponseType(typeof(List<RecommendationSimpleModel>))]
        public IHttpActionResult GetRecommendationSimplesSingleScenario([FromUri] Guid scenarioId)
        {
            return GetRecommendationSimplesMultipleScenarios(new List<Guid>() { scenarioId });
        }

        /// <summary>
        /// Gets simple recommendations for scenarios. Only Optimiser, ISR and
        /// Right Sizer recommendations are returned.
        /// </summary>
        /// <param name="scenarioIds"></param>
        [HttpGet]
        [Route("recommendations/simple")]
        [ResponseType(typeof(List<RecommendationSimpleModel>))]
        public IHttpActionResult GetRecommendationSimplesMultipleScenarios([FromUri] IEnumerable<Guid> scenarioIds)
        {
            if (GetInvalidScenarioIds(scenarioIds).Any())
            {
                return NotFound();
            }

            var recommendationSimpleModels = new List<RecommendationSimpleModel>();

            var recommendationSimples = _recommendationRepository.GetRecommendationSimplesByScenarioIdsAndProcessors(
                scenarioIds.ToList(), new List<string>() { "autobook", "isr", "rzr" });

            SetRecommendationProductInfo(ref recommendationSimples);

            recommendationSimpleModels.AddRange(_mapper.Map<List<RecommendationSimpleModel>>(recommendationSimples));

            return Ok(recommendationSimpleModels);
        }

        /// <summary>
        /// Returns ScenarioIds from list that do not exist
        /// </summary>
        /// <param name="scenarioIds"></param>
        /// <returns></returns>
        [HttpGet]
        private List<Guid> GetInvalidScenarioIds(IEnumerable<Guid> scenarioIds)
        {
            List<Guid> invalidScenarioIds = new List<Guid>();
            var runs = new List<Run>();
            foreach (Guid scenarioId in scenarioIds)
            {
                var run = runs.FirstOrDefault(r => r.Scenarios.FindIndex(s => s.Id == scenarioId) != -1);
                bool inMemory = (run != null);
                if (run == null)   // Not in memory, load it
                {
                    run = _runRepository.FindByScenarioId(scenarioId);
                }
                if (run == null)    // Doesn't exist
                {
                    invalidScenarioIds.Add(scenarioId);
                }
                else if (!inMemory)
                {
                    runs.Add(run);
                }
            }
            return invalidScenarioIds;
        }

        /// <summary>
        /// Returns aggregated recommedations for given scenario.
        /// </summary>
        /// <param name="scenarioId"></param>
        [HttpGet]
        [Route("{scenarioId}/recommendations/aggregate")]
        [ResponseType(typeof(List<RecommendationAggregateModel>))]
        public IHttpActionResult GetAggregatedReccomendations(Guid scenarioId)
        {
            var recommendationsAggregate = _recommendationAggregator.Aggregate(scenarioId);

            //Group based on CampaignGroup if present
            var input = recommendationsAggregate.Where(r => r.CampaignGroup != null).ToList();
            var recommendationsGrouped = input.GroupBy(r => r.CampaignGroup).Select(g => new RecommendationAggregateModel()
            {
                ExternalCampaignNumber = null,
                SpotRating = g.Sum(_ => _.SpotRating),
                CampaignGroup = g.Key,
                CampaignName = g.First().CampaignName,
                TargetRatings = g.Sum(_ => _.TargetRatings),
                ActualRatings = g.Sum(_ => _.ActualRatings),
                EndDateTime = g.Max(_ => _.EndDateTime),
                IsPercentage = g.First().IsPercentage,
                AdvertiserName = g.First().AdvertiserName
            }).ToList();

            //Find the campaigns for each campaign group that has not got the recommendataions
            var missingCampaigns = (input.Any() && recommendationsGrouped.Any())
                                                ? _campaignRepository.FindMissingCampaignsFromGroup(
                                                input.Select(i => i.ExternalCampaignNumber).ToList(),
                                                recommendationsGrouped.Select(r => r.CampaignGroup).ToList())?.ToList()
                                                : null;
            var missingCampaignGroups = missingCampaigns?.Select(x => x.CampaignGroup).ToHashSet() ?? new HashSet<string>();
            //Update the target and actual ratings from the campaigns that has not got recommendations
            if (missingCampaigns != null)
            {
                foreach (var recGroup in recommendationsGrouped.Where(r => missingCampaignGroups.Contains(r.CampaignGroup)))
                {
                    recGroup.TargetRatings += missingCampaigns.Where(c => c.CampaignGroup == recGroup.CampaignGroup)
                        .Sum(c => c.TargetRatings);
                    recGroup.ActualRatings += missingCampaigns.Where(c => c.CampaignGroup == recGroup.CampaignGroup)
                        .Sum(c => c.ActualRatings);
                }
            }

            //Output
            var output = recommendationsAggregate.Where(r => r.CampaignGroup == null).ToList();
            output.AddRange(recommendationsGrouped);
            return Ok(output);
        }

        /// <summary>
        /// Calcs metrics for scenario.
        /// </summary>
        /// <param name="scenarioId"></param>
        [HttpPost]
        [Route("{scenarioId}/metrics")]
        [ResponseType(typeof(IEnumerable<ScenarioResult>))]
        public IHttpActionResult PostMetrics(Guid scenarioId)
        {
            var scenarioResult = _scenarioResultRepository.Find(scenarioId);

            if (scenarioResult == null)
            {
                return NotFound();
            }

            var runId = _runRepository.GetRunIdForScenario(scenarioId);
            using (var calculationScope = _kpiCalculationScopeFactory.CreateCalculationScope(runId, scenarioId))
            {
                var kpiCalculationContext = calculationScope.Resolve<IKPICalculationContext>();
                var kpiCalculationManager = calculationScope.Resolve<IKPICalculationManager>();

                kpiCalculationContext.Recommendations = _recommendationRepository.GetByScenarioId(scenarioId);
                kpiCalculationContext.SetContextData(_failuresRepository.Get(scenarioId));

                scenarioResult.Metrics = kpiCalculationManager
                    .SetAudit(null)
                    .CalculateKPIs(runId, scenarioId);
            }

            return Ok(scenarioResult);
        }

        /// <summary>
        /// Gets failures for scenario
        /// </summary>
        /// <param name="scenarioId"></param>
        [HttpGet]
        [Route("{scenarioId}/failures")]
        [ResponseType(typeof(IEnumerable<FailureModel>))]
        public IHttpActionResult GetFailures(Guid scenarioId)
        {
            var failures = _failuresRepository.Get(scenarioId);
            if (failures == null)
            {
                return NotFound();
            }

            var items = GetFailureModels(failures.Items);
            return Ok(items);
        }

        /// <summary>
        /// Search scenario campaign failures
        /// </summary>
        /// <param name="queryModel"></param>
        [HttpPost]
        [Route("campaignfailures/search")]
        [ResponseType(typeof(SearchResultModel<ScenarioCampaignFailureModel>))]
        public IHttpActionResult GetScenarioCampaignFailures([FromBody] ScenarioCampaignFailureSearchQueryModel queryModel)
        {
            if (!ModelState.IsValid || queryModel == null)
            {
                return this.Error().InvalidParameters("One or more of the required query parameters are invalid.");
            }
            if (queryModel.ScenarioId == Guid.Empty)
            {
                return BadRequest("Invalid Scenario Id");
            }

            if (queryModel.StrikeWeights?.Count() > 0)
            {
                foreach (var strikeWeight in queryModel.StrikeWeights)
                {
                    strikeWeight.StrikeWeightStartDate = strikeWeight.StrikeWeightStartDate?.ToUniversalTime();
                    strikeWeight.StrikeWeightEndDate = strikeWeight.StrikeWeightEndDate?.ToUniversalTime();
                }
            }

            var scenarioCampaignFailures = _scenarioCampaignFailureRepository.Search(queryModel);
            var scenarioCampaignFailureModelList = ExtendScenarioCampaignFailureModel(scenarioCampaignFailures.Items.ToList());
            var searchModel = new SearchResultModel<ScenarioCampaignFailureModel>()
            {
                Items = scenarioCampaignFailureModelList.ToList(),
                TotalCount = scenarioCampaignFailures.TotalCount
            };

            return Ok(searchModel);
        }

        /// <summary>
        /// Get scenario's campaign failures stream for excel export by scenario id.
        /// </summary>
        /// <param name="scenarioId"></param>
        [HttpGet]
        [Route("{scenarioId}/campaignfailures/export/get-link")]
        [Route("{scenarioId}/campaignfailures/export/link")]
        public IHttpActionResult GetExportScenarioCampaignFailuresLink([FromUri] Guid scenarioId)
        {
            var engine = _storageClientFactory.GetEngine();

            var run = _runRepository.FindByScenarioId(scenarioId);
            if (run is null)
            {
                return this.Error().ResourceNotFound(RunNotFoundMessage);
            }

            var scenario = _scenarioRepository.Get(scenarioId);
            var filePath = ScenarioCampaignFailuresReportCreator.GetFilePath(scenario.Name, run.ExecuteStartedDateTime.Value, scenario.Id);

            var descriptor = new S3FileComment()
            {
                BucketName = _awsSettings.S3Bucket,
                FileNameWithPath = filePath
            };

            if (!engine.FileExists(descriptor))
            {
                var reportService = new ScenarioCampaignFailuresReportCreator();
                var reportData = GenerateScenarioCampaignFailuresDataShapshot(scenarioId);
                var tenantStartDayOfWeek = _tenantSettingsRepository.GetStartDayOfWeek();

                var excelFile = reportService.GenerateReport(reportData.data, reportData.snapshot, tenantStartDayOfWeek);

                engine.Upload(new S3UploadComment()
                {
                    BucketName = descriptor.BucketName,
                    DestinationFilePath = descriptor.FileNameWithPath,
                    FileStream = excelFile
                });
            }

            return Content(HttpStatusCode.OK, engine.GetPreSignedUrl(descriptor));
        }

        /// <summary>
        /// Get best scenario id
        /// </summary>
        /// <param name="id"></param>
        [HttpGet]
        [Route("best")]
        [ResponseType(typeof(Guid))]
        public IHttpActionResult GetBestScenario(Guid runId)
        {
            var run = _runRepository.Find(runId);

            if (run == null)
            {
                return NotFound();
            }

            if (!run.HasAllScenarioCompletedSuccessfully)
            {
                return BadRequest("Not all scenarios completed successfully");
            }

            var scenarioResults = _scenarioResultRepository.Find(run.Scenarios.Select(x => x.Id).ToArray());
            if (scenarioResults.All(x => x.BRSIndicator.HasValue))
            {
                var bestBRSIndicator = scenarioResults.Max(x => x.BRSIndicator.Value);
                return Ok(scenarioResults.First(x => x.BRSIndicator == bestBRSIndicator).Id);
            }

            return BadRequest("Not all scenario results contain BRS indicator value");
        }

        private List<FailureModel> GetFailureModels(IEnumerable<Failure> failures)
        {
            // Index sales areas by name
            var salesAreasByName = SalesArea.IndexListByName(_salesAreaRepository.GetAll());

            // Map Failure to FailureModel
            var failuresModels = _mapper.Map<List<FailureModel>>(failures);
            failuresModels.ForEach(fm => fm.SalesAreaShortName = salesAreasByName.ContainsKey(fm.SalesAreaName) ? salesAreasByName[fm.SalesAreaName].ShortName : fm.SalesAreaShortName);
            return failuresModels;
        }

        /// <summary>
        /// Gets a list of failures across a number of scenarios
        /// </summary>
        /// <param name="scenarioIds"></param>
        /// <returns></returns>
        private IEnumerable<int> GetFailureTypes(List<Guid> scenarioIds)
        {
            var failures = new HashSet<int>();
            foreach (var scenario in scenarioIds)
            {
                var scenarioFailures = _failuresRepository.Get(scenario);

                if (scenarioFailures is null)
                {
                    continue;
                }
                scenarioFailures.Items.ForEach(item => failures.Add(item.Type));
            }
            return failures.ToList();
        }
    }
}
