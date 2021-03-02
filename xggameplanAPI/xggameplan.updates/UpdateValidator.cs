using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using xggameplan.common.Extensions;

namespace xggameplan.Updates
{
    public static class UpdateValidator
    {
        public static AggregateException ValidateUpdateСonfiguration(UpdateConfiguration configuration)
        {
            var validationExceptions = new List<Exception>();

            validationExceptions.AddIfNotNull(
                ValidateTenantConnectionString(configuration.TenantConnectionStrings));

            validationExceptions.AddIfNotNull(
                ValidateMasterConnectionString(configuration.MasterConnectionString));

            validationExceptions.AddIfNotNull(
                ValidateUpdateFolderPath(configuration.UpdatesFolderRoot));

            if (validationExceptions.Any())
            {
                return new AggregateException(validationExceptions);
            }

            return null;
        }

        public static Exception ValidateTenantConnectionString(IEnumerable<string> tenantConnectionStrings, bool throwOnInvalid = false)
        {
            if (tenantConnectionStrings == null || !tenantConnectionStrings.Any() || tenantConnectionStrings.Any(cs => string.IsNullOrWhiteSpace(cs)))
            {
                return ThrowOrReturn(
                    exception: new ArgumentException("One or more provided Tenant Connection Strings is(are) invalid", nameof(tenantConnectionStrings)),
                    shouldThrow: throwOnInvalid);
            }

            return null;
        }

        public static Exception ValidateMasterConnectionString(string masterConnectionString, bool throwOnInvalid = false)
        {
            if (string.IsNullOrWhiteSpace(masterConnectionString))
            {
                return ThrowOrReturn(
                    exception: new ArgumentException("Provided Master Connection String is invalid", nameof(masterConnectionString)),
                    shouldThrow: throwOnInvalid);
            }

            return null;
        }

        public static Exception ValidateUpdateFolderPath(string updateFolder, bool throwOnInvalid = false)
        {
            try
            {
                _ = Path.GetFullPath(updateFolder);
            }
            catch (Exception ex)
            {
                return ThrowOrReturn(
                    exception: new ArgumentException("Provided Updates Folder Root is invalid", nameof(updateFolder), ex),
                    shouldThrow: throwOnInvalid);
            }

            return null;
        }

        private static Exception ThrowOrReturn(Exception exception, bool shouldThrow)
        {
            if (exception == null)
            {
                return null;
            }

            return !shouldThrow ? exception : throw exception;
        }
    }
}
