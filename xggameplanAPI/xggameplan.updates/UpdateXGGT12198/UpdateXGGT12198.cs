using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT12198 : UpdateBase, IUpdate
    {
        public UpdateXGGT12198(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT12198_AddKPIComparisonConfigurationsForNewKPIs(configuration.TenantConnectionStrings, _updatesFolder),
                new UpdateStepXGGT12198_AddNewKPIsToBRSTemplates(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid> { new Guid("4EF5C7D0-397D-4A6E-A4A8-1009DC9EFB0D") };

        public override Guid Id => new Guid("82F3AB06-D2B7-4753-8018-AF1187EA9F29");

        public override string Name => "XGGT-12198";

        public override string DatabaseVersion => "";
    }
}
