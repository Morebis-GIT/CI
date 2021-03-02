using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT12646 : UpdateBase, IUpdate
    {
        public UpdateXGGT12646(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT12646_RenameMaxRatingsRules(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public sealed override Guid Id => new Guid("df91644e-726a-4a44-984b-ce18c72dd3f1");

        public override string Name => "XGGT-12646";

        public override string DatabaseVersion { get; } = "";
    }
}
