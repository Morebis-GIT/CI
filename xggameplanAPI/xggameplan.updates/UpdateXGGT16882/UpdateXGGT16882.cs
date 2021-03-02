using System;
using System.Collections.Generic;
using System.IO;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT16882
{
    public class UpdateXGGT16882 : UpdateBase, IUpdate
    {
        public UpdateXGGT16882(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT16882_AddAnalysisGroupsFeatureFlag(configuration.MasterConnectionString, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("8168fc4b-1cd2-47db-ac8c-180d9d4efeaa");

        public override string Name => "XGGT-16882";

        public override string DatabaseVersion => "";
    }
}
