using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGP2119 : UpdateBase, IUpdate
    {
        private IEnumerable<string> _tenantConnectionStrings;

        public UpdateXGGP2119(UpdateConfiguration configuration)
        {
            _tenantConnectionStrings = configuration.TenantConnectionStrings;
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, this.Id.ToString());
            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>()
            {
                new UpdateXGGP2119_XGGP2119(_tenantConnectionStrings, _updatesFolder)
            };
        }

        public override Guid Id => new Guid("45adb6da-f649-48d2-ad3a-acb78d963d56");

        public override string Name => "XGGP-2119";

        public override string DatabaseVersion => "";

        public List<Guid> DependsOnUpdates => new List<Guid>();
    }
}
