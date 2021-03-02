using System;
using System.Collections.Generic;
using System.IO;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT12670
{
    public class UpdateXGGT12670 : UpdateBase, IUpdate
    {
        public UpdateXGGT12670(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT12670_AddSkySpecificFeatureFlags(configuration.MasterConnectionString, _updatesFolder)
            };
        }

        public override Guid Id => new Guid("7A41F113-2700-4F6F-A020-6D372895D7CA");

        public override string Name => "XGGT-12670";

        public override string DatabaseVersion => "";

        public List<Guid> DependsOnUpdates => new List<Guid>();
    }
}
