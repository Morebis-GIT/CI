using System;
using System.Collections.Generic;
using System.IO;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT_13218
{
    public class UpdateXGGT13218 : UpdateBase, IUpdate
    {
        public UpdateXGGT13218(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT13218_AddDataLoadFeatureFlag(configuration.MasterConnectionString, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("15B2765F-0097-42BF-AEAD-06764B47B532");

        public override string Name => "XGGT-13218";

        public override string DatabaseVersion => "";
    }
}
