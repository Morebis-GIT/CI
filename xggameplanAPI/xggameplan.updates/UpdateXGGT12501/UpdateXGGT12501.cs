using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT12501 : UpdateBase, IUpdate
    {
        public UpdateXGGT12501(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());
            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>()
            {
                new UpdateXGGT12501_AddOutputFile(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("b77f92ca-ddf8-4826-a3f1-57cc8a0c5888");

        public override string Name => "XGGP-12501";

        public override string DatabaseVersion => "";
    }
}
