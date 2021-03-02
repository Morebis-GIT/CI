using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.System.Models;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories;
using Raven.Abstractions.Data;
using Raven.Client;

namespace xggameplan.Updates
{
    /// <summary>
    /// Update step XGGP-752 - Converts the elements in Run documents of SalesAreas[] to SalesAreaPriorities[]
    /// </summary>
    internal class UpdateStepXGGP752_XGGP752 : CodeUpdateStepBase, IUpdateStep
    {
        //public IRepositoryFactory _repositoryFactory;

        private readonly List<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateStepXGGP752_XGGP752(List<string> tenantConnectionStrings, string updatesFolder)
        {
            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id
        {
            get { return new Guid("b948c643-2f4e-4c7a-b5aa-381d5b18ac01"); }
        }

        public int Sequence
        {
            get { return 1; }
        }

        public string Name
        {
            get { return "XGGP-752"; }
        }

        /// <summary>
        /// Converts runs to new format:
        /// salesareas converted to salesareaspriorities - defaulting to Priority3, if run salesareas field is empty then will be all sales areas Priority3
        /// Start\EndDateTime converted to Start\EndDate and Start\EndTime
        /// Process steps DateRange will be taken from run DateRange
        /// </summary>
        /// <param name="runOld"></param>
        /// <param name="runs"></param>
        private void ConvertRun(RunV2 runOld, List<Run> runs, List<SalesArea> allsalesareaDocuments)
        {
            if (runOld.StartDateTime != null && runOld.EndDateTime != null)
            {
                DateRange runOldDateRange = new DateRange { Start = runOld.StartDateTime.Date, End = runOld.EndDateTime.Date };
                Run run = new Run()
                {
                    Id = runOld.Id,
                    CustomId = runOld.CustomId,
                    Description = runOld.Description,
                    CreatedDateTime = runOld.CreatedDateTime,
                    StartDate = runOld.StartDateTime.Date,
                    EndDate = runOld.EndDateTime.Date,
                    StartTime = runOld.StartDateTime.TimeOfDay,
                    EndTime = runOld.EndDateTime.TimeOfDay,
                    LastModifiedDateTime = runOld.LastModifiedDateTime,
                    ExecuteStartedDateTime = runOld.ExecuteStartedDateTime,
                    IsLocked = runOld.IsLocked,
                    InventoryLock = runOld.InventoryLock,
                    Real = runOld.Real,

                    Smooth = runOld.Smooth,
                    SmoothDateRange = runOldDateRange,

                    ISR = runOld.ISR,
                    ISRDateRange = runOldDateRange,

                    Optimisation = runOld.Optimisation,
                    OptimisationDateRange = runOldDateRange,

                    RightSizer = runOld.RightSizer,
                    RightSizerDateRange = runOldDateRange,

                    SpreadProgramming = runOld.SpreadProgramming,
                    SkipLockedBreaks = runOld.SkipLockedBreaks,
                    IgnorePremiumCategoryBreaks = runOld.SkipLockedBreaks,
                    ExcludeBankHolidays = runOld.SkipLockedBreaks,
                    ExcludeSchoolHolidays = runOld.SkipLockedBreaks,
                    IgnoreZeroPercentageSplit = runOld.IgnoreZeroPercentageSplit,
                    BookTargetArea = runOld.BookTargetArea,
                    Objectives = runOld.Objectives,
                    Author = runOld.Author,

                    Campaigns = runOld.Campaigns,
                    Scenarios = runOld.Scenarios,

                    SalesAreaPriorities = GetSalesAreaPriorities(runOld, allsalesareaDocuments)

                    //RunStatus = runOld.RunStatus, //RunStatus property cannot be assigned, it is read only
                };
                runs.Add(run);
            }
        }

        //convert salesareas to salesareapriority
        private List<SalesAreaPriority> GetSalesAreaPriorities(RunV2 runOld, List<SalesArea> allsalesareaDocuments)
        {
            List<SalesAreaPriority> SalesAreaPriorities = new List<SalesAreaPriority>();
            //??? if (runOld.SalesAreas != null && runOld.SalesAreas.Count == 0)   //CHECK that salesarea is not null AND salesareas.count is 0 empty so set to allsalesareas defaulting to priority 3
            if (runOld.SalesAreas.Count == 0 || runOld.SalesAreas == null)   //salesareas is empty so set to allsalesareas defaulting to priority 3
            {
                foreach (var salesarea in allsalesareaDocuments)
                {
                    SalesAreaPriority salesAreaPriority = new SalesAreaPriority();
                    salesAreaPriority.SalesArea = salesarea.Name;
                    salesAreaPriority.Priority = SalesAreaPriorityType.Priority3;
                    SalesAreaPriorities.Add(salesAreaPriority);
                }
            }
            else
            {
                foreach (var salesareaCustomId in runOld.SalesAreas)
                {
                    SalesAreaPriority salesAreaPriority = new SalesAreaPriority();
                    salesAreaPriority.SalesArea = allsalesareaDocuments.First(s => s.CustomId.Equals(salesareaCustomId.Id)).Name;
                    salesAreaPriority.Priority = SalesAreaPriorityType.Priority3;
                    SalesAreaPriorities.Add(salesAreaPriority);
                }
            }
            return SalesAreaPriorities;
        }

        /// <summary>
        /// Converts Run documents from old format to new format where scenarios & passes are stored in separate document
        /// </summary>
        public void Apply()
        {
            //string targetTenantConnectionStringForTesting = "Url = http://18.130.103.86:8080;Database=ImagineDevChris";
            foreach (string tenantConnectionString in _tenantConnectionStrings)
            {
                using (IDocumentStore documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                {
                    using (IDocumentSession session = documentStore.OpenSession())
                    {
                        // get all the sales areas for use in converting run document
                        var salesAreaRepository = new RavenSalesAreaRepository(session);
                        var allsalesareas = salesAreaRepository.GetAll().ToList();

                        // Get all Run documents
                        var runDocuments = session.Advanced.DocumentStore.DatabaseCommands.Query("Raven/DocumentsByEntityName",
                                                                 new IndexQuery
                                                                 {
                                                                     Query = "Tag:[[Runs]]",
                                                                     PageSize = 1024
                                                                 });

                        List<Run> runs = new List<Run>();

                        foreach (var runDocument in runDocuments.Results)
                        {
                            string runJson = runDocument.ToString();
                            RunV2 runOld = Newtonsoft.Json.JsonConvert.DeserializeObject<RunV2>(runJson);

                            // Set RunID
                            var metadata = runDocument["@metadata"];
                            var idToken = metadata.SelectToken("@id");
                            if (idToken.ToString().Contains('/'))   // Should look like this: "@id": "runs/63afc561-f754-49b5-92ba-e984960466d5"
                            {
                                runOld.Id = new Guid(idToken.ToObject<string>().Split('/')[1]);
                            }
                            else                                    // Or like this: "@id": "63afc561-f754-49b5-92ba-e984960466d5" (if imported into raven from a csv file)
                            {
                                runOld.Id = new Guid(idToken.ToObject<string>());
                            }
                            // Back up Run document using run id as file name
                            if (!String.IsNullOrEmpty(_rollBackFolder))
                            {
                                string runFile = Path.Combine(_rollBackFolder, string.Format("Run.{0}.json", runOld.Id));
                                if (File.Exists(runFile))
                                {
                                    File.Delete(runFile);
                                }
                                File.WriteAllText(runFile, runJson);
                            }

                            // Convert run
                            ConvertRun(runOld, runs, allsalesareas);
                        }

                        using (IDocumentStore targetDocumentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                        {
                            using (IDocumentSession session1 = targetDocumentStore.OpenSession())
                            {
                                runs.ForEach(run => session1.Store(run));
                                session1.SaveChanges();
                            }
                        }
                    }
                }
            }
        }

        public bool SupportsRollback
        {
            get { return false; }
        }

        public void RollBack()
        {
            throw new NotImplementedException();
        }
    }
}
