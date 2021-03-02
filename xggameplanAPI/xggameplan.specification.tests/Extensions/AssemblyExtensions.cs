using System;
using System.Linq;
using System.Reflection;

namespace xggameplan.specification.tests.Extensions
{
    public static class AssemblyExtensions
    {
        public static string GetFullManifestResourceName(this Assembly assembly, string partialResourceName)
        {
            var resources = assembly.GetManifestResourceNames();
            return resources.FirstOrDefault(x => x == partialResourceName) ??
                   resources.FirstOrDefault(x => x.EndsWith(partialResourceName, StringComparison.InvariantCulture)) ??
                   throw new ArgumentException("The specified resource name doesn't exist.",
                       nameof(partialResourceName));
        }
    }
}
