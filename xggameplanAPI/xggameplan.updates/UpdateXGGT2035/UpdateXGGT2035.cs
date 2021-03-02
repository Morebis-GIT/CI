using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates.UpdateXGGT2035
{
    public class UpdateXGGT2035 : UpdateBase, IUpdate
    {
        public UpdateXGGT2035(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT2035_AddDefaultFacilities(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public override Guid Id => new Guid("9A3D39EB-8F9B-4E35-BBBD-617C9B4F92AB");

        public override string Name => "XGGT-2035";

        public override string DatabaseVersion => "";

        public List<Guid> DependsOnUpdates => new List<Guid>();
    }
}
