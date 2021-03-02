using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT8737 : UpdateBase, IUpdate
    {
        private IEnumerable<string> _tenantConnectionStrings;

        public UpdateXGGT8737(UpdateConfiguration configuration)
        {
            _tenantConnectionStrings = configuration.TenantConnectionStrings;
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, this.Id.ToString());
            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>()
            {
                new UpdateXGGT8737_CreateRule(_tenantConnectionStrings, _updatesFolder)
                ,new UpdateXGGT8737_UpdatePasses(_tenantConnectionStrings, _updatesFolder)
            };
        }

        public override Guid Id
        {            
            get { return new Guid("1E49DE04-AC17-4FAD-B596-10D53327157D"); }
        }

        public override string Name
        {
            get { return "XGGT-8737"; }
        }

        public override string DatabaseVersion
        {
            get { return ""; }    
        }

        public List<Guid> DependsOnUpdates
        {
            get { return new List<Guid>(); }
        }
    }
}
