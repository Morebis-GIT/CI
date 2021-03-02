using System;
using System.Collections.Generic;
using System.IO;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT13351
{
    public class UpdateXGGT13351 : UpdateBase, IUpdate
    {
        public UpdateXGGT13351(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT13351_AddSmoothFeatureFlag(configuration.MasterConnectionString, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("b423df67-3b65-4aee-81db-98ce196aa25d");

        public override string Name => "XGGT-13351";

        public override string DatabaseVersion => "";
    }
}
