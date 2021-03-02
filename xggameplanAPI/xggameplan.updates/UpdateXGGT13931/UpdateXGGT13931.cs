using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT13931 : UpdateBase, IUpdate
    {
        public UpdateXGGT13931(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT13931_AddNewKPIsToBRSTemplates(configuration.TenantConnectionStrings, _updatesFolder),
                new UpdateStepXGGT13931_AddKPIComparisonConfigurationsForNewKPIs(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid> { new Guid("4EF5C7D0-397D-4A6E-A4A8-1009DC9EFB0D") };

        public override Guid Id => new Guid("89953B63-FE6E-486B-9226-CA636A07BC03");

        public override string Name => "XGGT-13931";

        public override string DatabaseVersion => "";
    }
}
