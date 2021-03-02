using System;
using System.Collections.Generic;
using System.IO;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT10095
{
    public class UpdateXGGT10095 : UpdateBase, IUpdate
    {
        public UpdateXGGT10095(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT10095_AddSortIndexField(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("04A0BB53-6921-483E-BB76-1FEC4063143C");

        public override string Name => "XGGT-10095";

        public override string DatabaseVersion => "";
    }
}
