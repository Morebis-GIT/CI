using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGP1252 : UpdateBase, IUpdate
    {
        public UpdateXGGP1252(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, this.Id.ToString());
            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>()
            {
                new UpdateXGGP1252_AddDefaultEfficiencySettings(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("e6513be8-4a22-4dff-a0cb-ce8ddd380a9b");

        public override string Name => "XGGP-1252";

        public override string DatabaseVersion => "";
    }
}
