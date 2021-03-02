using System;
using System.Collections.Generic;
using System.IO;
using xggameplan.Updates;

namespace xggameplan.updates
{
    public class UpdateXGGT12543 : UpdateBase, IUpdate
    {
        public UpdateXGGT12543(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateXGGT12543_AddRunTypeFeatureFlag(configuration.MasterConnectionString, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("8d2b8c95-57ea-425f-8c4e-8d8c5bb05701");

        public override string Name => "XGGT-12543";

        public override string DatabaseVersion => "";
    }
}
