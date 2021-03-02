using System;
using System.Collections.Generic;
using System.IO;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT18062
{
    public class UpdateXGGT18062 : UpdateBase, IUpdate
    {
        public UpdateXGGT18062(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT18062_TypoInFailureDescription(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("b5a72ad7-8f8c-44e0-ad4f-2e76daf959e2");

        public override string Name => "XGGT-18062";

        public override string DatabaseVersion => "";
    }
}
