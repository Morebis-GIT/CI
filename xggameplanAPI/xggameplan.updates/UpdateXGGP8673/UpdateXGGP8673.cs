using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGP8673 : UpdateBase, IUpdate
    {
        public UpdateXGGP8673(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, this.Id.ToString());
            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>()
            {
                new UpdateXGGP8673_AddOutputFile(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("b77f92ca-ddf8-4826-a3f1-57cc8a0c5f80");

        public override string Name => "XGGP-8673";

        public override string DatabaseVersion => "";
    }
}
