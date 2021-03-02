using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace xggameplan.TestEnv
{
    public class TestEnvironment : ITestEnvironment
    {
        private const StringComparison TestEnvOptionStringComparison = StringComparison.InvariantCultureIgnoreCase;

        private readonly IConfiguration _configuration;
        private readonly List<TestEnvironmentFeature> _supportFeatures;

        private bool _initialized;
        private TestEnvironmentOptions _mode = TestEnvironmentOptions.None;
        private readonly ICollection<string> _features = new List<string>();

        protected virtual string GetTestEnvironmentModeValue()
        {
            return _configuration["Environment:TestEnvironmentMode"];
        }

        private static void CheckTestEnvironmentOptions()
        {
            var pairedOptionsList = GetDescriptiveOptions().ToList();

            var fieldAndDescDuplicates = pairedOptionsList
                .Where(x => !x.Key.Equals(x.Value.ToString(), TestEnvOptionStringComparison))
                .Any(x => pairedOptionsList.Any(i => x.Key.Equals(i.Value.ToString(), TestEnvOptionStringComparison)));
            if (fieldAndDescDuplicates)
            {
                throw new Exception(
                    $"Names of '{typeof(TestEnvironmentOptions).Name}' fields and their descriptions have the same names.");
            }
        }

        protected static IEnumerable<KeyValuePair<string, TestEnvironmentOptions>> GetDescriptiveOptions()
        {
            var enumType = typeof(TestEnvironmentOptions);
            return Enum.GetValues(enumType).Cast<TestEnvironmentOptions>()
                .SelectMany(enumField =>
                {
                    var nameOfField = enumField.ToString();
                    var value = new List<KeyValuePair<string, TestEnvironmentOptions>>
                    {
                        new KeyValuePair<string, TestEnvironmentOptions>(nameOfField, enumField)
                    };
                    var nameFromDesc = enumType.GetField(nameOfField)
                        ?.GetCustomAttribute<DescriptionAttribute>(false)?.Description;
                    if (!string.IsNullOrWhiteSpace(nameFromDesc) &&
                        !nameFromDesc.Equals(nameOfField, TestEnvOptionStringComparison))
                    {
                        value.Add(new KeyValuePair<string, TestEnvironmentOptions>(nameFromDesc, enumField));
                    }

                    return value;

                });
        }

        protected void CheckTestEnvironmentFeatures()
        {
            if (_supportFeatures.Any())
            {
                var featureDuplicates =
                    _supportFeatures
                        .Select(f => f.Name)
                        .Distinct(new TestEnvironmentFeatureNameEqualityComparer(TestEnvOptionStringComparison))
                        .Count() != _supportFeatures.Count;
                if (featureDuplicates)
                {
                    throw new Exception("Test environment features contain duplicates.");
                }

                var wrongFeatureNames = _supportFeatures.Where(a =>
                        GetDescriptiveOptions().Any(i => i.Key.Equals(a.Name, TestEnvOptionStringComparison)))
                    .Select(a => a.Name).ToList();
                if (wrongFeatureNames.Any())
                {
                    throw new Exception(
                        $"Test environment feature(s) '{string.Join(", ", wrongFeatureNames)}' can't be defined because '{typeof(TestEnvironmentOptions).Name}' type items has the same name(s).");
                }
            }
        }

        protected void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            lock (this)
            {
                if (_initialized)
                {
                    return;
                }

                var modeValue = GetTestEnvironmentModeValue();
                if (!string.IsNullOrWhiteSpace(modeValue))
                {
                    var options = modeValue.Split(new[] {'+'}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(o => o.Trim())
                        .ToArray();

                    if (options.Any())
                    {
                        var pairedOptionsList = GetDescriptiveOptions().ToList();
                        foreach (var option in options)
                        {
                            var idx = pairedOptionsList.FindIndex(p =>
                                p.Key.Equals(option, TestEnvOptionStringComparison));
                            if (idx != -1)
                            {
                                _mode |= pairedOptionsList[idx].Value;
                            }
                            else
                            {
                                idx = _supportFeatures.FindIndex(f =>
                                    f.Name.Equals(option, TestEnvOptionStringComparison));
                                if (idx != -1)
                                {
                                    _mode |= _supportFeatures[idx].Options;
                                    _features.Add(option);
                                }
                            }
                        }
                    }
                }

                _initialized = true;
            }
        }

        static TestEnvironment()
        {
            CheckTestEnvironmentOptions();
        }

        public TestEnvironment(IConfiguration configuration, params TestEnvironmentFeature[] supportedFeatures)
        {
            _configuration = configuration;
            _supportFeatures = supportedFeatures.ToList();
            CheckTestEnvironmentFeatures();
        }

        public TestEnvironmentOptions Mode
        {
            get
            {
                Initialize();
                return _mode;
            }
        }

        public bool Enabled => Mode > TestEnvironmentOptions.None;

        public bool HasOptions(TestEnvironmentOptions options)
        {
            return Mode.HasFlag(options);
        }

        public bool HasFeature(string featureName)
        {
            Initialize();
            return _features.Any(f => f.Equals(featureName, TestEnvOptionStringComparison));
        }

        public void Reload()
        {
            lock (this)
            {
                _mode = TestEnvironmentOptions.None;
                _features.Clear();
                _initialized = false;
            }
        }
    }

    internal class TestEnvironmentFeatureNameEqualityComparer : IEqualityComparer<string>
    {
        private readonly StringComparison _comparisonType;

        public TestEnvironmentFeatureNameEqualityComparer(StringComparison comparisonType)
        {
            _comparisonType = comparisonType;
        }

        public bool Equals(string x, string y)
        {
            return string.Equals(x, y, _comparisonType);
        }

        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }
}
