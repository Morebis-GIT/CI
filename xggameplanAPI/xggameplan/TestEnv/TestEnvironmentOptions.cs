using System;
using System.ComponentModel;

namespace xggameplan.TestEnv
{
    [Flags]
    public enum TestEnvironmentOptions
    {
        [Description("null")]
        None = 0,
        WaitForIndexes = 1,
        ExceptionFilter = 2,
        PerformanceLog = 4,
        AutoBookStub = 8,
        LandmarkServicesStub = 16
    }
}
