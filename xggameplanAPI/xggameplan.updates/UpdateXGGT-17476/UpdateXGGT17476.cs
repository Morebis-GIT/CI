using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT17476 : UpdateBase, IUpdate
    {
        public UpdateXGGT17476(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT17476_AddSystemLogicalDateFeatureFlag(configuration.MasterConnectionString, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("a1a8cab3-752a-4136-b6ce-55eccb0a6f45");

        public override string Name => "XGGT-17476";

        public override string DatabaseVersion => "";
    }
}
