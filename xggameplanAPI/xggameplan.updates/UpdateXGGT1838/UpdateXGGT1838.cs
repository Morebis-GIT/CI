using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT1838 : UpdateBase, IUpdate
    {
        public UpdateXGGT1838(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT1838_FunctionalAreaAndFaultTypeInsert(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public sealed override Guid Id => new Guid("6872877a-51a0-4ecd-acd0-0203e4727abe");

        public override string Name => "XGGT-1838";

        public override string DatabaseVersion { get; } = "";
    }
}
