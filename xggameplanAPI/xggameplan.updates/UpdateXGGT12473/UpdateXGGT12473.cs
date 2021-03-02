using System;
using System.Collections.Generic;
using System.IO;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT12473
{
    public class UpdateXGGT12473 : UpdateBase, IUpdate
    {
        public UpdateXGGT12473(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT12473_AddShortNameForFaultTypes(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("FA9107C3-09AC-49C3-BC8C-F2FCC7D86458");

        public override string Name => "XGGT-12473";

        public override string DatabaseVersion => "";
    }
}
