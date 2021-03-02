using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT14208 : UpdateBase, IUpdate
    {
        public UpdateXGGT14208(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT14208_AddScenarioPerformanceMeasurementKPIsProductFeature(configuration.MasterConnectionString, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("8b231e65-15de-4c16-a566-7120957e9857");

        public override string Name => "XGGT-14208";

        public override string DatabaseVersion => "";
    }
}
