using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates.UpdateXGGT7762
{
    public class UpdateXGGT7762 : UpdateBase, IUpdate
    {
        public UpdateXGGT7762(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT7762_AddDefaultPiBEntities(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public override Guid Id => new Guid("C7C05DF2-24C9-436E-9CE1-2CB5BDEB86DC");

        public override string Name => "XGGT-7762";

        public override string DatabaseVersion => "";

        public List<Guid> DependsOnUpdates => new List<Guid>();
    }
}
