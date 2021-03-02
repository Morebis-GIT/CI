using System;
using System.Collections.Generic;
using AutoFixture;
using AutoMapper;
using ImagineCommunications.GamePlan.CommonTestsHelpers;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using xggameplan.Profile;

namespace ImagineCommunications.GamePlan.Core.Tests
{
    public abstract class DataPopulationTestBase
    {
        protected internal Fixture _fixture { get; }

        protected internal IMapper _mapper { get; set; }

        protected internal Random _random { get; set; }

        protected internal Run _run { get; set; }

        protected internal List<Pass> _passes { get; set; }

        protected internal Scenario _scenario { get; set; }

        protected internal List<SalesArea> _salesAreas { get; set; }

        protected internal TenantSettings _tenantSettings { get; set; }

        protected internal AgExposure _agExposure => new AgExposure();

        protected internal string[] _salesAreaArr = { "QTQ92", "STW94", "TCN91", "STW93", "STW92", "STW91", "TCN94", "GTV91", "TCN92", "TCN93", "GTV94", "GTV92", "NWS92", "NWS94", "NWS91", "QTQ94", "QTQ93", "QTQ91", "NWS93", "GTV93" };

        protected DataPopulationTestBase()
        {
            _fixture = new SafeFixture();
            _mapper = new Mapper(new MapperConfiguration(expression => expression.AddProfile<ClashProfile>()));
            _random = new Random();
        }
    }
}
