using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGP1418 : UpdateBase, IUpdate
    {
        public UpdateXGGP1418(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, this.Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>()
            {
                new UpdateStepXGGP1418_RightSizerLevelPropertyPatch(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id
        {
            get { return new Guid("9e64b874-0d29-4d9b-97df-c569f13c1440"); }
        }

        public override string Name
        {
            get { return "XGGP-1418"; }
        }

        public override string DatabaseVersion
        {
            get { return ""; } 
        }
    }
}
