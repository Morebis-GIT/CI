using System;
using System.Collections.Generic;
using System.IO;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT17536
{
    public class UpdateXGGT17536 : UpdateBase, IUpdate
    {
        public UpdateXGGT17536(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT17536_AddCampaignResultsProcessingFeatureFlag(configuration.MasterConnectionString, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("12713788-eb63-4479-a78a-ec978d7d0c37");

        public override string Name => "XGGT-17536";

        public override string DatabaseVersion => "";
    }
}
