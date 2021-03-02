using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Newtonsoft.Json;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Json.Linq;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGP2022_MapStartAndEnd : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateStepXGGP2022_MapStartAndEnd(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            ValidateParametersBeforeUse(tenantConnectionStrings, updatesFolder);

            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("56597c99-f7c0-47a3-b2d8-f1e3b11e542f");

        public void Apply()
        {
            foreach (string tenantConnectionString in _tenantConnectionStrings)
            {
                using (IDocumentStore documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                using (IDocumentSession session = documentStore.OpenSession())
                {
                    var runDocuments = session.Advanced.DocumentStore.DatabaseCommands.Query(
                        "Raven/DocumentsByEntityName",
                        new IndexQuery
                        {
                            Query = "Tag:[[Runs]]",
                            PageSize = 1024
                        });

                    Backup(runDocuments.Results);

                    ClearTimes(runDocuments.Results, session);

                    var dateTimeMaps = runDocuments.Results.Select(ParseQueryResult).ToList();

                    var runs = session.GetAll<Run>();

                    foreach (var run in runs)
                    {
                        var relatedMap = dateTimeMaps.FirstOrDefault(m => m.runId == run.Id);

                        const double defaultSalesAreaOffset = 6d;

                        run.StartDate = relatedMap.startDateTime.Date.AddHours(defaultSalesAreaOffset);
                        run.EndDate = relatedMap.endDateTime.Date.AddHours(defaultSalesAreaOffset);

                        run.StartTime = ExtractTime(relatedMap.startDateTime);
                        run.EndTime = ExtractTime(relatedMap.endDateTime);
                    }

                    session.SaveChanges();
                }
            }
        }

        public void RollBack()
        {
            throw new NotImplementedException();
        }

        public int Sequence => 1;

        public string Name => "XGGP-2022 : Map Start And End";

        public bool SupportsRollback => false;

        private void ValidateParametersBeforeUse(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _ = UpdateValidator.ValidateTenantConnectionString(tenantConnectionStrings, throwOnInvalid: true);
            _ = UpdateValidator.ValidateUpdateFolderPath(updatesFolder, throwOnInvalid: true);
        }

        private void Backup(List<RavenJObject> currentRuns)
        {
            foreach (var run in currentRuns)
            {
                var runId = GetId(run);
                var runJson = run.ToString();

                if (!String.IsNullOrEmpty(_rollBackFolder))
                {
                    string runFile = Path.Combine(_rollBackFolder, string.Format("Run.{0}.json", runId));
                    if (File.Exists(runFile))
                    {
                        File.Delete(runFile);
                    }
                    File.WriteAllText(runFile, runJson);
                }
            }
        }

        private void ClearTimes(List<RavenJObject> currentRuns, IDocumentSession session)
        {
            foreach (var run in currentRuns)
            {
                _ = run.Remove("StartTime");
                _ = run.Remove("EndTime");

                var parsed = JsonConvert.DeserializeObject<Run>(run.ToString());
                parsed.Id = GetId(run);

                session.Store(parsed);
            }

            session.SaveChanges();
        }

        private (Guid runId, DateTime startDateTime, DateTime endDateTime) ParseQueryResult(RavenJObject obj)
        {
            var runId = GetId(obj);

            var startDateTime = obj.ContainsKey("StartDateTime")
                ? obj.Value<DateTime>("StartDateTime")
                : throw new InvalidOperationException("StartDateTime could not be parsed");

            var endDateTime = obj.ContainsKey("EndDateTime")
                ? obj.Value<DateTime>("EndDateTime")
                : throw new InvalidOperationException("EndDateTime could not be parsed");

            return (runId: runId, startDateTime: startDateTime, endDateTime: endDateTime);
        }

        private Guid GetId(RavenJObject obj)
        {
            var metadata = obj["@metadata"];
            var idToken = metadata.SelectToken("@id");
            return new Guid(idToken.ToObject<string>().Split('/')[1]);
        }

        private TimeSpan ExtractTime(DateTime dateTime)
        {
            return dateTime.TimeOfDay;
        }
    }
}
