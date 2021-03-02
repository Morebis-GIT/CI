using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT9644 : UpdateBase, IUpdate
    {
        public UpdateXGGT9644(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateXGGT9644_RemoveClashDifferences(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public sealed override Guid Id => new Guid("930CF035-675C-49BC-9853-9E8061767317");

        public override string Name => "XGGT-9644";

        public override string DatabaseVersion { get; } = "";
    }
}
