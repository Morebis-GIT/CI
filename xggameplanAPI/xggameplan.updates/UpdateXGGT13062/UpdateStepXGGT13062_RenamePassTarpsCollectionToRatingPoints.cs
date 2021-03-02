using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using Raven.Abstractions.Data;
using xggameplan.Updates;
using xggameplan.updates.UpdateXGGT13062_Models;

namespace xggameplan.updates
{
    internal class UpdateStepXGGT13062_RenamePassTarpsCollectionToRatingPoints : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateStepXGGT13062_RenamePassTarpsCollectionToRatingPoints(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("8EF77788-304A-491E-ADF1-2D6ECAC0C797");
        public string Name => "XGGT-13062: Rename Tarps collection of pass to RatingPoints";
        public int Sequence => 1;
        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString, null))
                using (var session = documentStore.OpenSession())
                {
                    var passesOld = new List<PassV1>();

                    var page = 0;
                    var pageSize = 1024;

                    QueryResult passQueryResult = null;

                    do
                    {
                        passQueryResult = session.Advanced.DocumentStore.DatabaseCommands.Query("Raven/DocumentsByEntityName",
                            new IndexQuery { Query = "Tag:[[Passes]]", Start = page * pageSize, PageSize = pageSize });

                        ParseRavenDocuments(passQueryResult, passesOld);

                        page++;

                    } while (page * pageSize <= (passQueryResult?.TotalResults ?? 0));

                    var passesNew = passesOld.Select(UpdatePassStructure).ToList();

                    passesNew.ForEach(pass => session.Store(pass));

                    session.SaveChanges();
                }
            }
        }

        private void ParseRavenDocuments(QueryResult documents, List<PassV1> passes)
        {
            foreach (var passDocument in documents.Results)
            {
                var passJson = passDocument.ToString();
                var passOld = Newtonsoft.Json.JsonConvert.DeserializeObject<PassV1>(passJson);

                var metadata = passDocument["@metadata"];
                var idToken = metadata.SelectToken("@id");
                if (idToken.ToString().Contains('/'))   // Should look like this: "@id": "passes/111"
                {
                    passOld.Id = Convert.ToInt32(idToken.ToObject<string>().Split('/')[1]);
                }
                else                                    // Or like this: "@id": "111"
                {
                    passOld.Id = Convert.ToInt32(idToken.ToObject<string>());
                }

                passes.Add(passOld);
            }
        }

        private Pass UpdatePassStructure(PassV1 passOld)
        {
            return new Pass
            {
                Id = passOld.Id,
                Name = passOld.Name,
                DateCreated = passOld.DateCreated,
                DateModified = passOld.DateModified,
                IsLibraried = passOld.IsLibraried,
                General = passOld.General,
                Weightings = passOld.Weightings,
                Tolerances = passOld.Tolerances,
                Rules = passOld.Rules,
                RatingPoints = passOld.Tarps.Select(ConvertTarpToRatingPoint).ToList(),
                ProgrammeRepetitions = passOld.ProgrammeRepetitions,
                BreakExclusions = passOld.BreakExclusions,
                SlottingLimits = passOld.SlottingLimits,
                PassSalesAreaPriorities = passOld.PassSalesAreaPriorities
            };
        }

        private RatingPoint ConvertTarpToRatingPoint(TarpV1 tarp)
        {
            return new RatingPoint
            {
                Id = tarp.Id,
                SalesAreas = tarp.SalesAreas,
                OffPeakValue = tarp.OffPeakValue,
                PeakValue = tarp.PeakValue,
                MidnightToDawnValue = tarp.MidnightToDawnValue
            };
        }

        public bool SupportsRollback => false;
        public void RollBack() => throw new NotImplementedException();
    }
}
