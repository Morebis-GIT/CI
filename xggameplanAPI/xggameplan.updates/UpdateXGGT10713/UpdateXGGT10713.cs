using System;
using System.Collections.Generic;
using System.IO;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT10713
{
    public class UpdateXGGT10713 : UpdateBase, IUpdate
    {
        public UpdateXGGT10713(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT10713_AddSkipLockedBreaks(configuration.MasterConnectionString, _updatesFolder)
            };
        }

        public override Guid Id => new Guid("451EB2E0-325F-4437-BED9-A0099D85A767");

        public override string Name => "XGGT-10713";

        public override string DatabaseVersion => "";

        public List<Guid> DependsOnUpdates => new List<Guid>();
    }
}
