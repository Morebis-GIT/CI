using System;

namespace xggameplan.TestEnv
{
    public struct TestEnvironmentFeature
    {
        public TestEnvironmentFeature(string name, TestEnvironmentOptions options)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (options == TestEnvironmentOptions.None)
            {
                throw new ArgumentException($"'{name}' test environment feature options are not defined.");
            }

            Name = name;
            Options = options;
        }

        public string Name { get; }

        public TestEnvironmentOptions Options { get; }
    }
}
