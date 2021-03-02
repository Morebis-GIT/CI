using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGP1253 : UpdateBase, IUpdate
    {
        public UpdateXGGP1253(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, this.Id.ToString());
            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>()
            {
                new UpdateXGGP1253_EfficiencyCalculationPeriodRunDefaults(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("0b766e18-bc08-459e-863f-31185430c5a9");

        public override string Name => "XGGP-1253";

        public override string DatabaseVersion => "";
    }
}
