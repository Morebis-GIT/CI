using System;
using System.Collections.Generic;
using System.IO;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT18424
{
    public class UpdateXGGT18424 : UpdateBase, IUpdate
    {
        public UpdateXGGT18424(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT18424_FailuresAreShownWithoutNames(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("e79adf0c-455a-4475-8be5-b0bf518e5fa6");

        public override string Name => "XGGT-18424";

        public override string DatabaseVersion => "";
    }
}
