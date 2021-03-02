using System;
using System.Collections.Generic;
using System.IO;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT9240
{
    public class UpdateXGGT9240 : UpdateBase, IUpdate
    {
        public UpdateXGGT9240(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());
            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT9240_AddLengthFactors(configuration.MasterConnectionString, _updatesFolder)
            };
        }

        public override Guid Id => new Guid("C4039746-C287-4FA9-8EBF-A367CA357EFF");

        public override string Name => "XGGT-9240";

        public override string DatabaseVersion => "";

        public List<Guid> DependsOnUpdates => new List<Guid>();
    }
}
