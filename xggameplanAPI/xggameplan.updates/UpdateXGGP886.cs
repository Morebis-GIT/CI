using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace xggameplan.Updates
{
    /// <summary>
    /// Update for XGGP-886
    /// </summary>
    public class UpdateXGGP886 : UpdateBase, IUpdate
    {
        private List<string> _tenantConnectionStrings;
       
        public UpdateXGGP886(UpdateConfiguration configuration)
        {
            _tenantConnectionStrings = configuration.TenantConnectionStrings.ToList();            
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, this.Id.ToString());
            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>()
            {
                new UpdateStepXGGP886_XGGP886(_tenantConnectionStrings, _updatesFolder)
            };                        
        }

        public override Guid Id
        {
            get { return new Guid("3f231c7a-6233-435d-b857-4903a396291a"); }
        }

        public override string Name
        {
            get { return "XGGP-886"; }
        }

        public override string DatabaseVersion
        {
            get { return ""; }    // TODO: Set this
        }

        public List<Guid> DependsOnUpdates
        {
            get { return new List<Guid>(); }
        }
    }
}
