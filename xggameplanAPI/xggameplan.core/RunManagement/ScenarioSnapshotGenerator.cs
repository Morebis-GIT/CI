using System;
using xggameplan.common.Types;
using xggameplan.AutoBooks.Abstractions;
using xggameplan.Model;
using AutoBookDomainObject = ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage.Objects.AutoBook;

namespace xggameplan.core.RunManagement
{
    public class ScenarioSnapshotGenerator
    {
        private readonly RootFolder _rootFolder;
        private readonly IAutoBooks _autoBooks;

        public ScenarioSnapshotGenerator(RootFolder rootFolder, IAutoBooks autoBooks)
        {
            _rootFolder = rootFolder;
            _autoBooks = autoBooks;
        }

        public void Generate(AutoBookDomainObject autoBook, Guid scenarioId)
        {
            //string snapshotFile = string.Format(@"{0}\{1}.snapshot.zip", System.Web.Hosting.HostingEnvironment.MapPath("/Output"), _scenarioId);
            string snapshotFile = string.Format(@"{0}\Output\{1}.snapshot.zip", _rootFolder, scenarioId);
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(snapshotFile));
            if (System.IO.File.Exists(snapshotFile))
            {
                System.IO.File.Delete(snapshotFile);
            }

            IAutoBook autoBookInterface = _autoBooks.GetInterface(autoBook);
            GetAutoBookSnapshotModel autoBookSnapshot = autoBookInterface.GetSnapshot(scenarioId);
            System.IO.File.WriteAllBytes(snapshotFile, autoBookSnapshot.Data);
        }
    }
}
