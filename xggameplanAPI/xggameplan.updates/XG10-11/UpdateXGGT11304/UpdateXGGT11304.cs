using System;
using System.Collections.Generic;
using System.IO;
using xggameplan.Updates;

namespace xggameplan.updates.XG10_11.UpdateXGGT11304
{
    public class UpdateXGGT11304 : UpdateBase, IUpdate
    {
        public UpdateXGGT11304(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateXGGT11304_AddSponsorshipSmoothFailures(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public sealed override Guid Id => new Guid("BFF2D6B8-B326-4F65-9DD1-57A9110F6F6E");

        public override string Name => "XGGT-11304";

        public override string DatabaseVersion => "";

    }
}
