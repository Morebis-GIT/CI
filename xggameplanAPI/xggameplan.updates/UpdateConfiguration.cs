using System.Collections.Generic;
using AutoMapper;

namespace xggameplan.Updates
{
    /// <summary>
    /// Configuration class for all updates
    /// </summary>
    public class UpdateConfiguration
    {
        public IEnumerable<string> TenantConnectionStrings { get; }
        public string MasterConnectionString { get; }

        /// <summary>
        /// Folder where files for updates are stored
        /// </summary>
        public string UpdatesFolderRoot { get; }

        public IMapper Mapper { get; }

        public UpdateConfiguration(IEnumerable<string> tenantConnectionStrings, string masterConnectionString, string updatesFolderRoot, IMapper mapper)
        {
            TenantConnectionStrings = tenantConnectionStrings;
            MasterConnectionString = masterConnectionString;
            UpdatesFolderRoot = updatesFolderRoot;
            Mapper = mapper;

            _ = UpdateValidator.ValidateUpdateСonfiguration(this);
        }
    }
}
