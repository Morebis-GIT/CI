using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    internal class UpdateXGGT4293 : UpdateBase, IUpdate
    {
        public List<Guid> DependsOnUpdates => new List<Guid>();

        public sealed override Guid Id => new Guid("40eaa147-23b7-4c44-ba04-0dac23e49864");

        public override string Name => "XGGT-4293";

        public override string DatabaseVersion { get; } = "";

        public UpdateXGGT4293(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT4293_SetAutoBookDefaultParameters(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }
    }
}
