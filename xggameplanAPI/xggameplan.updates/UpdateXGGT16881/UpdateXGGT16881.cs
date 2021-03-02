using System;
using System.Collections.Generic;
using System.IO;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT16881
{
    public class UpdateXGGT16881 : UpdateBase, IUpdate
    {
        public UpdateXGGT16881(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT16881_AddAnalysisGroupsFeatureFlag(configuration.MasterConnectionString, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("0145ec69-697f-47aa-a8e7-d8e61c5e0582");

        public override string Name => "XGGT-16881";

        public override string DatabaseVersion => "";
    }
}
