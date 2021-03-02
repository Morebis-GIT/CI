using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{ 
    public class UpdateXGGT7590 : UpdateBase, IUpdate
    {
        public UpdateXGGT7590(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT7590_RemoveExternalIdentifierValue(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public sealed override Guid Id => new Guid("09eba2ea-e143-4a46-a0e4-91711f80b886");

        public override string Name => "XGGT-7590";

        public override string DatabaseVersion { get; } = "";
    }
}
