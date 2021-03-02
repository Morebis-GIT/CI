using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Spots;
using xggameplan.AuditEvents;
using xggameplan.AutoBooks;
using xggameplan.CSVImporter;
using xggameplan.OutputFiles.Processing;
using xggameplan.OutputProcessors.Abstractions;
using xggameplan.Repository;
using xggameplan.Repository.CSV;

namespace xggameplan.core.OutputProcessors.Processors
{
    public class SpotsReqmOutputFileProcessor : IOutputFileProcessor<SpotsReqmOutput>
    {
        private readonly IOutputDataSnapshot _dataSnapshot;
        private readonly IAuditEventRepository _audit;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IMapper _mapper;

        public SpotsReqmOutputFileProcessor(
            IOutputDataSnapshot dataSnapshot,
            IAuditEventRepository auditEventRepository,
            ICampaignRepository campaignRepository,
            IMapper mapper)
        {
            _dataSnapshot = dataSnapshot;
            _audit = auditEventRepository;
            _campaignRepository = campaignRepository;
            _mapper = mapper;
        }

        public string FileName { get; } = OutputFileNames.Spot;

        public SpotsReqmOutput ProcessFile(Guid scenarioId, string folder)
        {
            string spotFile = FileHelpers.GetPathToFileIfExists(folder, FileName);
            string mainSpotFile = FileHelpers.GetPathToFileIfExists(folder, OutputFileNames.MainSpotTable);
            var output = new SpotsReqmOutput
            {
                ScenarioId = scenarioId
            };

            if (String.IsNullOrEmpty(spotFile) || String.IsNullOrEmpty(mainSpotFile))
            {
                return output;
            }

            _audit.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                0, $"Processing output file {spotFile}"));

            _audit.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                0, $"Processing output file {mainSpotFile}"));

            // Process file
            DateTime processorDateTime = DateTime.UtcNow;

            var importSettings = CSVImportSettings.GetImportSettings(mainSpotFile, typeof(SpotHeaderMap), typeof(SpotIndexMap));

            // Get main spots if path passed in
            ISpotImportRepository spotImportRepository = new CSVSpotImportRepository(importSettings);
            IEnumerable<SpotImport> spotImports = spotImportRepository.GetAll();

            importSettings = CSVImportSettings.GetImportSettings(spotFile, typeof(SpotReqmHeaderMap), typeof(SpotReqmIndexMap));

            ISpotReqmImportRepository spotReqmImportRepository = new CSVSpotReqmImportRepository(importSettings);
            var spotReqmImports = spotReqmImportRepository.GetAll();

            // Index for lookup performance
            var campaignsByCustomId = CampaignReducedModel.IndexListByCustomId(_campaignRepository.GetAllFlat());
            var spotsByCustomId = SpotHelper.IndexListById(_dataSnapshot.SpotsForRun.Value);
            var demographicsById = Demographic.IndexListById(_dataSnapshot.AllDemographics.Value);
            var programmeDictionariesById = ProgrammeDictionary.IndexListById(_dataSnapshot.AllProgrammeDictionaries.Value);

            IEnumerable<Pass> scenarioPasses = _dataSnapshot.ScenarioPasses.Value;

            foreach (var currentSalesArea in _dataSnapshot.AllSalesAreas.Value)
            {
                foreach (SpotReqmImport spotReqmImport in spotReqmImports.Where(sri => sri.sare_no == currentSalesArea.CustomId))
                {
                    output.CountTotalSpots++;

                    try
                    {
                        Recommendation recommendation = _mapper.Map<Recommendation>(Tuple.Create(spotReqmImport, new List<ProgrammeDictionary>(), _dataSnapshot.BreakTypes.Value));
                        recommendation.ScenarioId = scenarioId;
                        recommendation.ProcessorDateTime = processorDateTime;

                        bool isCancelledSpot = !String.IsNullOrEmpty(spotReqmImport.status) && spotReqmImport.status == "C";

                        if (isCancelledSpot)
                        {
                            output.CountCancelledSpots++;
                            recommendation.SpotSequenceNumber = default;

                            if (spotsByCustomId.TryGetValue((int)spotReqmImport.spot_no, out var spot))
                            {
                                recommendation.ExternalSpotRef = spot.ExternalSpotRef;
                                recommendation.Sponsored = spot.Sponsored;
                                recommendation.Preemptable = spot.Preemptable;
                                recommendation.Preemptlevel = spot.Preemptlevel;
                                recommendation.MultipartSpot = spot.MultipartSpot;
                                recommendation.MultipartSpotPosition = spot.MultipartSpotPosition;
                                recommendation.MultipartSpotRef = spot.MultipartSpotRef;
                            }
                            else
                            {
                                _audit.Insert(
                                    AuditEventFactory.CreateAuditEventForWarningMessage(
                                        0,
                                        0,
                                        $"Unable to set cancelled spot details for recommendations" +
                                        $" because spot {spotReqmImport.spot_no} does not exist")
                                    );
                            }
                        }
                        else
                        {
                            recommendation.SpotSequenceNumber = spotReqmImport.spot_no;
                        }

                        Pass pass;

                        if (scenarioPasses != null && scenarioPasses.Any() && (pass = scenarioPasses.FirstOrDefault(p => p.Id == spotReqmImport.abdn_no)) != null)
                        {
                            recommendation.PassName = pass.Name;
                            recommendation.OptimiserPassSequenceNumber = spotReqmImport.pass_sequence_no;
                        }

                        recommendation.CampaignPassPriority = spotReqmImport.campaign_pass_priority;
                        recommendation.RankOfEfficiency = spotReqmImport.rank_of_efficiency;
                        recommendation.RankOfCampaign = spotReqmImport.rank_of_campaign;
                        recommendation.CampaignWeighting = spotReqmImport.campaign_weighting;

                        recommendation.SalesArea = recommendation.GroupCode = currentSalesArea.Name;

                        recommendation.NominalPrice = spotReqmImport.nominal_price;

                        recommendation.ExternalCampaignNumber = campaignsByCustomId.TryGetValue((int)spotReqmImport.camp_no, out var campaign)
                            ? campaign.ExternalId
                            : string.Empty;

                        recommendation.Demographic = demographicsById.TryGetValue(spotReqmImport.demo_no, out var demographic)
                            ? demographic.ExternalRef
                            : string.Empty;

                        if (programmeDictionariesById.TryGetValue((int)spotReqmImport.prog_no, out var programmeDictionary))
                        {
                            recommendation.ExternalProgrammeReference = programmeDictionary.ExternalReference;
                            recommendation.ProgrammeName = programmeDictionary.ProgrammeName;
                        }
                        else
                        {
                            recommendation.ExternalProgrammeReference = recommendation.ProgrammeName = string.Empty;
                        }

                        recommendation.ClientPicked = spotReqmImport.client_picked.ToUpper() == "Y";
                        recommendation.ExternalBreakNo = spotReqmImport.brek_external_ref;

                        output.Recommendations.Add(recommendation);
                    }
                    catch (System.Exception exception)
                    {
                        // Log exception, continue
                        _audit.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, string.Format("Error generating recommendation for spot no {0}", spotReqmImport.spot_no), exception));
                    }
                }
            }

            // TODO: Remove this when it has been confirmed that cancellations appear in Spot Req file
            // Process cancelled spots if they weren't present in the Spot Req
            // file above
            if (output.CountCancelledSpots == 0)
            {
                string scheduleIndexKey = string.Empty;

                var scheduleBreaksBySalesAreaAndDate = _dataSnapshot
                    .BreaksForRun
                    .Value
                    .GroupBy(k => GetScheduleIndexKey(k.SalesAreaName, k.ScheduleDate.Date))
                    .ToDictionary(c => c.Key, c => c.ToDictionary(v => v.CustomId));

                _audit.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0, string.Format("Processing {0} for cancellations because no cancelled spots were found in {1}", Path.GetFileName(mainSpotFile), Path.GetFileName(spotFile))));

                foreach (var currentSalesArea in _dataSnapshot.AllSalesAreas.Value)
                {
                    foreach (SpotImport spotImport2 in spotImports.Where(s => s.status == "C" && s.sare_no == currentSalesArea.CustomId))
                    {
                        try
                        {
                            scheduleIndexKey = string.Empty;
                            bool isCancelledSpot = !String.IsNullOrEmpty(spotImport2.status) && spotImport2.status == "C";    // Should all be cancelled

                            // Update statistics for Spot Performance, Campaign
                            // Performance, New Efficiency, cancelled spots
                            // should be excluded
                            output.CountTotalSpots++;
                            if (isCancelledSpot)
                            {
                                output.CountCancelledSpots++;
                            }

                            // Create recommendation
                            Recommendation recommendation = _mapper.Map<Recommendation>(Tuple.Create(spotImport2, new List<ProgrammeDictionary>()));
                            recommendation.ScenarioId = scenarioId;
                            recommendation.ProcessorDateTime = processorDateTime;

                            if (spotsByCustomId.TryGetValue((int)spotImport2.spot_no, out var spot))
                            {
                                recommendation.Sponsored = spot.Sponsored;
                                recommendation.Preemptable = spot.Preemptable;
                                recommendation.Preemptlevel = spot.Preemptlevel;
                                recommendation.ExternalSpotRef = spot.ExternalSpotRef;
                                recommendation.MultipartSpot = spot.MultipartSpot;
                                recommendation.MultipartSpotPosition = spot.MultipartSpotPosition;
                                recommendation.MultipartSpotRef = spot.MultipartSpotRef;
                            }
                            else
                            {
                                _audit.Insert(
                                        AuditEventFactory.CreateAuditEventForWarningMessage(
                                                0,
                                                0,
                                                $"Unable to set multipart spot details for recommendations because spot {spotImport2.spot_no} does not exist"
                                            )
                                    );
                            }

                            recommendation.SalesArea = recommendation.GroupCode = currentSalesArea.Name;

                            if (campaignsByCustomId.TryGetValue((int)spotImport2.camp_no, out CampaignReducedModel campaign))
                            {
                                recommendation.ExternalCampaignNumber = campaign.ExternalId;
                            }

                            if (demographicsById.TryGetValue(spotImport2.demo_no, out Demographic demographic))
                            {
                                recommendation.Demographic = demographic.ExternalRef;
                            }

                            scheduleIndexKey = GetScheduleIndexKey(recommendation.SalesArea, recommendation.StartDateTime.Date);

                            string breakNotSetReason = "Unknown reason";

                            if (spotImport2 == null)
                            {
                                breakNotSetReason = $"Spot not found in {Path.GetFileName(mainSpotFile)}";
                            }
                            else
                            {
                                if (!scheduleBreaksBySalesAreaAndDate.TryGetValue(scheduleIndexKey, out var scheduleBreakCache))
                                {
                                    breakNotSetReason = string.Format("Schedule document not found (SalesArea={0}, StartDateTime={1})", recommendation.SalesArea, recommendation.StartDateTime.ToString());
                                }
                                else
                                {
                                    recommendation.ExternalBreakNo = !scheduleBreakCache.TryGetValue(spotImport2.break_no, out var @break)
                                        ? recommendation.ExternalBreakNo
                                        : @break.ExternalBreakRef;

                                    // Set reason why ExternalBreakNo isn't set
                                    if (String.IsNullOrEmpty(recommendation.ExternalBreakNo))
                                    {
                                        if (!(scheduleBreakCache?.Any() ?? false))
                                        {
                                            breakNotSetReason = $"Schedule document exists but it contains no breakss (BreakNo={spotImport2.break_no}, Breaks=0)";
                                        }
                                        else if (@break == null)
                                        {
                                            breakNotSetReason = $"Schedule document exists but the break was not found (BreakNo={spotImport2.break_no}, Breaks={scheduleBreakCache.Count})";
                                        }
                                        else    // Break found but ExternalBreakRef is not set
                                        {
                                            breakNotSetReason = $"Schedule document exists and the break was found but the ExternalBreakRef is not set (BreakNo={spotImport2.break_no}, Breaks={scheduleBreakCache.Count})";
                                        }
                                    }
                                }

                                recommendation.ClientPicked = (spotImport2.client_picked.ToUpper() == "Y");
                            }

                            if (String.IsNullOrEmpty(recommendation.ExternalBreakNo))
                            {
                                _audit.Insert(
                                        AuditEventFactory.CreateAuditEventForWarningMessage(
                                                0,
                                                0,
                                                $"Unable to determine External Break No for recommendation for spot no {spotImport2.spot_no}: {breakNotSetReason}"
                                            )
                                    );
                            }

                            output.Recommendations.Add(recommendation);
                        }
                        catch (Exception exception)
                        {
                            // Log exception, continue
                            _audit.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, string.Format("Error generating cancellation recommendation for spot no {0}", spotImport2.spot_no), exception));
                        }
                    }
                }
            }

            _audit.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                0, $"Processed output file {spotFile}"));

            _audit.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                0, $"Processed output file {mainSpotFile}"));

            return output;
        }

        object IOutputFileProcessor.ProcessFile(Guid scenarioId, string folder) => ProcessFile(scenarioId, folder);

        private static string GetScheduleIndexKey(string salesArea, DateTime scheduleDate) => $"{salesArea}#{scheduleDate.Date:d}";
    }
}
