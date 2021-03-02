using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.DbSequence;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.OutputFiles;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Recommendations;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Domain.ResultsFiles;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Settings;
using ImagineCommunications.GamePlan.Domain.ScenarioFailures;
using ImagineCommunications.GamePlan.Domain.ScenarioFailures.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.System.Models;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using NodaTime;
using xggameplan.core.Interfaces;
using xggameplan.core.TestEnvironment;
using xggameplan.Model;

namespace xggameplan.core.Services
{
    public class TestEnvironmentDataService : ITestEnvironmentDataService
    {
        private static readonly string[] DowPattern = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

        private readonly IClock _clock;
        private readonly IDemographicRepository _demographicRepository;
        private readonly IClashRepository _clashRepository;
        private readonly IProductRepository _productRepository;
        private readonly ISalesAreaRepository _salesAreaRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IPassRepository _passRepository;
        private readonly IScenarioRepository _scenarioRepository;
        private readonly IRunRepository _runRepository;
        private readonly IScenarioResultRepository _scenarioResultRepository;
        private readonly IFailuresRepository _failuresRepository;
        private readonly IProgrammeRepository _programmeRepository;
        private readonly IBreakRepository _breakRepository;
        private readonly IRecommendationRepository _recommendationRepository;
        private readonly ISmoothConfigurationRepository _smoothConfigurationRepository;
        private readonly IResultsFileRepository _resultsFileRepository;
        private readonly IOutputFileRepository _outputFileRepository;
        private readonly IIdentityGeneratorResolver _identityGeneratorResolver;
        private readonly Random _randomizer = new Random((int)DateTime.Now.Ticks);

        public TestEnvironmentDataService(IClock clock,
            IDemographicRepository demographicRepository, IClashRepository clashRepository,
            ISalesAreaRepository salesAreaRepository, IProductRepository productRepository,
            ICampaignRepository campaignRepository, IPassRepository passRepository,
            IScenarioRepository scenarioRepository, IRunRepository runRepository,
            IScenarioResultRepository scenarioResultRepository, IFailuresRepository failuresRepository,
            IProgrammeRepository programmeRepository, IBreakRepository breakRepository,
            IRecommendationRepository recommendationRepository,
            ISmoothConfigurationRepository smoothConfigurationRepository,
            IResultsFileRepository resultsFileRepository, IOutputFileRepository outputFileRepository,
            IIdentityGeneratorResolver identityGeneratorResolver)
        {
            _clock = clock;
            _demographicRepository = demographicRepository;
            _clashRepository = clashRepository;
            _salesAreaRepository = salesAreaRepository;
            _productRepository = productRepository;
            _campaignRepository = campaignRepository;
            _passRepository = passRepository;
            _scenarioRepository = scenarioRepository;
            _runRepository = runRepository;
            _scenarioResultRepository = scenarioResultRepository;
            _failuresRepository = failuresRepository;
            _programmeRepository = programmeRepository;
            _breakRepository = breakRepository;
            _recommendationRepository = recommendationRepository;
            _smoothConfigurationRepository = smoothConfigurationRepository;
            _resultsFileRepository = resultsFileRepository;
            _outputFileRepository = outputFileRepository;
            _identityGeneratorResolver = identityGeneratorResolver;
        }

        public TestEnvironmentRunResult PopulateRunResult(TestEnvironmentOutputFilesInfo outputFilesResult)
        {
            var demographics = CreateDemographics(2);
            _demographicRepository.Add(demographics);
            _demographicRepository.SaveChanges();

            var salesAreas = CreateSalesAreas(3, demographics);
            foreach(var salesArea in salesAreas)
            {
                _salesAreaRepository.Add(salesArea);
            }
            _salesAreaRepository.SaveChanges();

            var pass = CreatePass(salesAreas);
            _passRepository.Add(pass);
            _passRepository.SaveChanges();

            var clash = CreateClash();
            _clashRepository.Add(clash);
            _clashRepository.SaveChanges();

            var product = CreateProduct(clash);
            _productRepository.Add(product);
            _productRepository.SaveChanges();

            var campaigns = CreateCampaigns(3, demographics.First(), product, salesAreas);
            _campaignRepository.Add(campaigns);
            _campaignRepository.SaveChanges();


            var scenario = CreateScenario(campaigns, pass, product, clash);
            _scenarioRepository.Add(scenario);
            _scenarioRepository.SaveChanges();

            var run = CreateRun(scenario, salesAreas);
            _runRepository.Add(run);
            _runRepository.SaveChanges();

            var scenarioResult = CreateScenarioResult(scenario);
            _scenarioResultRepository.Add(scenarioResult);
            _scenarioResultRepository.SaveChanges();

            var failures = CreateFailures(scenario, campaigns);
            _failuresRepository.Add(failures);
            _failuresRepository.SaveChanges();

            var programmes = CreateProgrammes(salesAreas);
            _programmeRepository.Add(programmes);
            _programmeRepository.SaveChanges();

            var breaks = CreateBreaks(salesAreas);
            _breakRepository.Add(breaks);
            _breakRepository.SaveChanges();

            var recommendations = CreateRecommendations(campaigns, scenario, pass, programmes, breaks);
            _recommendationRepository.Insert(recommendations);
            _recommendationRepository.SaveChanges();

            var resultFileInfo = CreateResultFiles(outputFilesResult, scenario.Id);

            return new TestEnvironmentRunResult
            {
                RunId = run.Id,
                ScenarioId = scenario.Id,
                CampaignCount = campaigns.Length,
                RecommendationCount = recommendations.Length,
                FileNamesAndLengths = resultFileInfo
            };
        }

        public int PopulateSmoothConfiguration()
        {
            var smoothConfiguration = _smoothConfigurationRepository.GetById(SmoothConfiguration.DefaultId);
            if (smoothConfiguration == null)
            {
                var smoothConfigurationBuilder = new SmoothConfigurationBuilder();
                smoothConfiguration = smoothConfigurationBuilder.Create();
                _smoothConfigurationRepository.Add(smoothConfiguration);
            }
            return smoothConfiguration.Id;
        }

        #region Documents creation methods

        private Demographic[] CreateDemographics(int count)
        {
            return Enumerable.Repeat(0, count).Select(x => new Demographic
            {
                Name = $"{nameof(Demographic.Name)}{Guid.NewGuid()}",
                ShortName = $"{nameof(Demographic.ShortName)}{Guid.NewGuid()}",
                ExternalRef = $"DRUN_{Guid.NewGuid()}",
                DisplayOrder = _randomizer.Next(),
                Gameplan = true,
            }).ToArray();
        }

        private Clash CreateClash()
        {
            var extRef = $"СRUN_{Guid.NewGuid()}";
            return new Clash
            {
                Uid = Guid.NewGuid(),
                Externalref = extRef,
                ParentExternalidentifier = extRef,
                Description = $"{nameof(Clash.Description)}{Guid.NewGuid()}",
                DefaultOffPeakExposureCount = 2
            };
        }

        private Product CreateProduct(Clash clash)
        {
            var utcDate = _clock.GetCurrentInstant().ToDateTimeUtc();
            return new Product
            {
                Uid = Guid.NewGuid(),
                Externalidentifier = _randomizer.Next().ToString(CultureInfo.InvariantCulture),
                Name = $"{nameof(Product.Name)}{Guid.NewGuid()}",
                EffectiveStartDate = utcDate.Date,
                EffectiveEndDate = utcDate.AddYears(1).AddDays(1).Date.AddMinutes(-1).Date,
                ClashCode = clash.Externalref,
                AdvertiserIdentifier = _randomizer.Next().ToString(CultureInfo.InvariantCulture),
                AdvertiserName = $"{nameof(Product.AdvertiserName)}{Guid.NewGuid()}"
            };
        }

        private SalesArea[] CreateSalesAreas(int count, Demographic[] demographics)
        {
            var currentYear = _clock.GetCurrentInstant().ToDateTimeUtc().ToLocalTime().Year;
            return Enumerable.Repeat(0, count).Select(x => new Func<SalesArea>(() =>
            {
                var name = $"{nameof(SalesArea)}{nameof(SalesArea.Name)}{Guid.NewGuid()}";
                return new SalesArea
                {
                    ChannelGroup = new List<string> { name },
                    Name = name,
                    ShortName = $"{nameof(SalesArea)}{nameof(SalesArea.ShortName)}{Guid.NewGuid()}",
                    CurrencyCode = "AUD",
                    BaseDemographic1 = demographics.FirstOrDefault()?.ExternalRef,
                    BaseDemographic2 = demographics.LastOrDefault()?.ExternalRef,
                    StartOffset = Duration.FromHours(6),
                    DayDuration = Duration.FromDays(1),
                    PublicHolidays = new List<DateRange>
                    {
                        new DateRange(new DateTime(currentYear, 6, 14), new DateTime(currentYear, 6, 17)),
                        new DateRange(new DateTime(currentYear, 7, 1), new DateTime(currentYear, 7, 5)),
                        new DateRange(new DateTime(currentYear, 12, 24), new DateTime(currentYear, 12, 26)),
                        new DateRange(new DateTime(currentYear, 12, 31), new DateTime(currentYear + 1, 1, 1))
                    },
                    SchoolHolidays = new List<DateRange>
                    {
                        new DateRange(new DateTime(currentYear, 6, 25), new DateTime(currentYear, 6, 30)),
                        new DateRange(new DateTime(currentYear, 7, 22), new DateTime(currentYear, 7, 31))
                    }
                };
            })()).ToArray();
        }

        private Campaign[] CreateCampaigns(int count, Demographic demographic, Product product, SalesArea[] salesAreas)
        {
            var utcDate = _clock.GetCurrentInstant().ToDateTimeUtc();
            return Enumerable.Repeat(0, count).Select(x => new Campaign
            {
                Id = Guid.NewGuid(),
                ExternalId = $"{nameof(Campaign.ExternalId)}{Guid.NewGuid()}",
                Name = $"{nameof(Campaign)}{nameof(Campaign.Name)}{Guid.NewGuid()}",
                DemoGraphic = demographic.ExternalRef,
                StartDateTime = utcDate.Date,
                EndDateTime = utcDate.Date.AddDays(6),
                Product = product.Externalidentifier,
                Status = "A",
                BusinessType = "Dynamic",
                DeliveryType = CampaignDeliveryType.Rating,
                IncludeOptimisation = true,
                TargetZeroRatedBreaks = false,
                InefficientSpotRemoval = true,
                IncludeRightSizer = true,
                RightSizerLevel = RightSizerLevel.CampaignLevel,
                CampaignPassPriority = 3,
                BreakType = new List<string> { "Base" },
                ProgrammeRestrictions = new List<ProgrammeRestriction>
                {
                    new ProgrammeRestriction
                    {
                        SalesAreas = null,
                        CategoryOrProgramme = new List<string> {"CHILDREN"},
                        IsCategoryOrProgramme = "C",
                        IsIncludeOrExclude = "E"
                    }
                },
                SalesAreaCampaignTarget = salesAreas.Select(sa => new SalesAreaCampaignTarget
                {
                    SalesArea = null,
                    SalesAreaGroup = new SalesAreaGroup
                    {
                        SalesAreas = new List<string> { sa.Name },
                        GroupName = sa.Name
                    },
                    Multiparts = new List<Multipart>
                    {
                        new Multipart
                        {
                            Lengths = new List<MultipartLength>
                            {
                                new MultipartLength
                                {
                                    Length = Duration.FromSeconds(15)
                                }
                            }
                        }
                    },
                    CampaignTargets = new List<CampaignTarget>
                    {
                        new CampaignTarget
                        {
                            StrikeWeights = new List<StrikeWeight>
                            {
                                new StrikeWeight
                                {
                                    StartDate = utcDate.Date,
                                    EndDate = utcDate.Date.AddDays(3),
                                    Lengths = new List<Length>
                                    {
                                        new Length
                                        {
                                            length = Duration.FromSeconds(15)
                                        }
                                    },
                                    DayParts = new List<DayPart>
                                    {
                                        new DayPart
                                        {
                                            Timeslices = new List<Timeslice>
                                            {
                                                new Timeslice
                                                {
                                                    FromTime = "18:00",
                                                    ToTime = "22:29",
                                                    DowPattern = DowPattern.ToList()
                                                }
                                            }
                                        },
                                        new DayPart
                                        {
                                            Timeslices = new List<Timeslice>
                                            {
                                                new Timeslice
                                                {
                                                    FromTime = "06:00",
                                                    ToTime = "17:59",
                                                    DowPattern = DowPattern.ToList()
                                                }
                                            }
                                        },
                                        new DayPart
                                        {
                                            Timeslices = new List<Timeslice>
                                            {
                                                new Timeslice
                                                {
                                                    FromTime = "22:30",
                                                    ToTime = "23:59",
                                                    DowPattern = DowPattern.ToList()
                                                }
                                            }
                                        }
                                    }
                                },
                                new StrikeWeight
                                {
                                    StartDate = utcDate.Date.AddDays(3),
                                    EndDate = utcDate.Date.AddDays(6),
                                    Lengths = new List<Length>
                                    {
                                        new Length
                                        {
                                            length = Duration.FromSeconds(15)
                                        }
                                    },
                                    DayParts = new List<DayPart>
                                    {
                                        new DayPart
                                        {
                                            Timeslices = new List<Timeslice>
                                            {
                                                new Timeslice
                                                {
                                                    FromTime = "18:00",
                                                    ToTime = "22:29",
                                                    DowPattern = DowPattern.ToList()
                                                }
                                            }
                                        },
                                        new DayPart
                                        {
                                            Timeslices = new List<Timeslice>
                                            {
                                                new Timeslice
                                                {
                                                    FromTime = "06:00",
                                                    ToTime = "17:59",
                                                    DowPattern = DowPattern.ToList()
                                                }
                                            }
                                        },
                                        new DayPart
                                        {
                                            Timeslices = new List<Timeslice>
                                            {
                                                new Timeslice
                                                {
                                                    FromTime = "22:30",
                                                    ToTime = "23:59",
                                                    DowPattern = DowPattern.ToList()
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }).ToList()

            }).ToArray();
        }

        private Pass CreatePass(SalesArea[] salesAreas)
        {
            var utcDate = _clock.GetCurrentInstant().ToDateTimeUtc();
            return new Pass
            {
                Id = _identityGeneratorResolver.Resolve<IPassRepository>()
                    .GetIdentities<PassIdIdentity>(1).First().Id,
                Name = $"{nameof(Pass)}{nameof(Pass.Name)}{Guid.NewGuid()}",
                IsLibraried = false,
                General = new List<General>
                {
                    new General
                    {
                        RuleId = 1,
                        InternalType = "Defaults",
                        Description = "Minimum Efficiency",
                        Value = "80",
                        Type = "general"
                    },
                    new General
                    {
                        RuleId = 2,
                        InternalType = "Defaults",
                        Description = "Maximum Rank",
                        Value = "99999",
                        Type = "general"
                    },
                    new General
                    {
                        RuleId = 3,
                        InternalType = "Defaults",
                        Description = "Demograph Banding Tolerance",
                        Value = "0",
                        Type = "general"
                    },
                    new General
                    {
                        RuleId = 4,
                        InternalType = "Defaults",
                        Description = "Default Centre Break Ratio",
                        Value = "50",
                        Type = "general"
                    },
                    new General
                    {
                        RuleId = 5,
                        InternalType = "Defaults",
                        Description = "Use Max Spot Ratings Set By Campaigns",
                        Value = "1",
                        Type = "general"
                    }
                },
                Weightings = new List<Weighting>
                {
                    new Weighting
                    {
                        RuleId = 1,
                        InternalType = "Weightings",
                        Description = "Campaign",
                        Value = "15",
                        Type = "weightings"
                    },
                    new Weighting
                    {
                        RuleId = 2,
                        InternalType = "Weightings",
                        Description = "Campaign Sales Area",
                        Value = "85",
                        Type = "weightings"
                    },
                    new Weighting
                    {
                        RuleId = 3,
                        InternalType = "Weightings",
                        Description = "Spot Length",
                        Value = "75",
                        Type = "weightings"
                    },
                    new Weighting
                    {
                        RuleId = 4,
                        InternalType = "Weightings",
                        Description = "Daypart",
                        Value = "65",
                        Type = "weightings"
                    },
                    new Weighting
                    {
                        RuleId = 5,
                        InternalType = "Weightings",
                        Description = "Strike Weight",
                        Value = "55",
                        Type = "weightings"
                    },
                    new Weighting
                    {
                        RuleId = 6,
                        InternalType = "Weightings",
                        Description = "Fit to Requirement",
                        Value = "87",
                        Type = "weightings"
                    },
                    new Weighting
                    {
                        RuleId = 7,
                        InternalType = "Weightings",
                        Description = "Fit to Spot Lengt",
                        Value = "22",
                        Type = "weightings"
                    },
                    new Weighting
                    {
                        RuleId = 8,
                        InternalType = "Weightings",
                        Description = "Centre/End",
                        Value = "0",
                        Type = "weightings"
                    },
                    new Weighting
                    {
                        RuleId = 9,
                        InternalType = "Weightings",
                        Description = "Booking Position",
                        Value = "0",
                        Type = "weightings"
                    }
                },
                Tolerances = new List<Tolerance>
                {
                    new Tolerance
                    {
                        RuleId = 1,
                        InternalType = "Campaign",
                        Description = "Campaign",
                        Value = null,
                        Under = 0,
                        Over = 5,
                        Ignore = false,
                        BookTargetArea = false,
                        ForceOverUnder = ForceOverUnder.None,
                        CampaignType = CampaignDeliveryType.Rating,
                        Type = "tolerances"
                    },
                    new Tolerance
                    {
                        RuleId = 8,
                        InternalType = "Campaign",
                        Description = "Booking Position",
                        Value = null,
                        Under = 0,
                        Over = 0,
                        Ignore = false,
                        BookTargetArea = false,
                        ForceOverUnder = ForceOverUnder.None,
                        CampaignType = CampaignDeliveryType.Rating,
                        Type = "tolerances"
                    },
                    new Tolerance
                    {
                        RuleId = 9,
                        InternalType = "Campaign",
                        Description = "Centre/End",
                        Value = null,
                        Under = 0,
                        Over = 0,
                        Ignore = false,
                        BookTargetArea = false,
                        ForceOverUnder = ForceOverUnder.None,
                        CampaignType = CampaignDeliveryType.Rating,
                        Type = "tolerances"
                    },
                    new Tolerance
                    {
                        RuleId = 2,
                        InternalType = "Sales Area",
                        Description = "Sales Area",
                        Value = null,
                        Under = 0,
                        Over = 5,
                        Ignore = false,
                        BookTargetArea = false,
                        ForceOverUnder = ForceOverUnder.None,
                        CampaignType = CampaignDeliveryType.Rating,
                        Type = "tolerances"
                    },
                    new Tolerance
                    {
                        RuleId = 3,
                        InternalType = "Sales Area",
                        Description = "Spot Length",
                        Value = null,
                        Under = 0,
                        Over = 5,
                        Ignore = false,
                        BookTargetArea = false,
                        ForceOverUnder = ForceOverUnder.None,
                        CampaignType = CampaignDeliveryType.Rating,
                        Type = "tolerances"
                    },
                    new Tolerance
                    {
                        RuleId = 4,
                        InternalType = "Sales Area",
                        Description = "Daypart",
                        Value = null,
                        Under = 0,
                        Over = 5,
                        Ignore = false,
                        BookTargetArea = false,
                        ForceOverUnder = ForceOverUnder.None,
                        CampaignType = CampaignDeliveryType.Rating,
                        Type = "tolerances"
                    },
                    new Tolerance
                    {
                        RuleId = 5,
                        InternalType = "Sales Area",
                        Description = "Strike Weight",
                        Value = null,
                        Under = 0,
                        Over = 5,
                        Ignore = false,
                        BookTargetArea = false,
                        ForceOverUnder = ForceOverUnder.None,
                        CampaignType = CampaignDeliveryType.Rating,
                        Type = "tolerances"
                    },
                    new Tolerance
                    {
                        RuleId = 6,
                        InternalType = "Sales Area",
                        Description = "Strike Wgt/Daypart",
                        Value = null,
                        Under = 0,
                        Over = 5,
                        Ignore = false,
                        BookTargetArea = false,
                        ForceOverUnder = ForceOverUnder.None,
                        CampaignType = CampaignDeliveryType.Rating,
                        Type = "tolerances"
                    },
                    new Tolerance
                    {
                        RuleId = 7,
                        InternalType = "Sales Area",
                        Description = "Peak Daypart",
                        Value = null,
                        Under = 0,
                        Over = 0,
                        Ignore = false,
                        BookTargetArea = false,
                        ForceOverUnder = ForceOverUnder.None,
                        CampaignType = CampaignDeliveryType.Rating,
                        Type = "tolerances"
                    },
                    new Tolerance
                    {
                        RuleId = 12,
                        InternalType = "Sales Area",
                        Description = "Daypart/Length",
                        Value = null,
                        Under = 0,
                        Over = 5,
                        Ignore = false,
                        BookTargetArea = false,
                        ForceOverUnder = ForceOverUnder.None,
                        CampaignType = CampaignDeliveryType.Rating,
                        Type = "tolerances"
                    },
                    new Tolerance
                    {
                        RuleId = 13,
                        InternalType = "Sales Area",
                        Description = "Strike Weight/Length",
                        Value = null,
                        Under = 0,
                        Over = 0,
                        Ignore = true,
                        BookTargetArea = false,
                        ForceOverUnder = ForceOverUnder.None,
                        CampaignType = CampaignDeliveryType.Rating,
                        Type = "tolerances"
                    },
                    new Tolerance
                    {
                        RuleId = 14,
                        InternalType = "Sales Area",
                        Description = "Strike Weight/Daypart/Length",
                        Value = null,
                        Under = 0,
                        Over = 0,
                        Ignore = true,
                        BookTargetArea = false,
                        ForceOverUnder = ForceOverUnder.None,
                        CampaignType = CampaignDeliveryType.Rating,
                        Type = "tolerances"
                    },
                    new Tolerance
                    {
                        RuleId = 10,
                        InternalType = "Budget",
                        Description = "Campaign (Budget)",
                        Value = null,
                        Under = 0,
                        Over = 0,
                        Ignore = true,
                        BookTargetArea = false,
                        ForceOverUnder = ForceOverUnder.None,
                        CampaignType = CampaignDeliveryType.Rating,
                        Type = "tolerances"
                    },
                    new Tolerance
                    {
                        RuleId = 11,
                        InternalType = "Budget",
                        Description = "Sales Area (Budget)",
                        Value = null,
                        Under = 0,
                        Over = 0,
                        Ignore = true,
                        BookTargetArea = false,
                        ForceOverUnder = ForceOverUnder.None,
                        CampaignType = CampaignDeliveryType.Rating,
                        Type = "tolerances"
                    },
                    new Tolerance
                    {
                        RuleId = 19,
                        InternalType = "Campaign",
                        Description = "Campaign",
                        Value = null,
                        Under = 0,
                        Over = 5,
                        Ignore = false,
                        ForceOverUnder = ForceOverUnder.None,
                        CampaignType = CampaignDeliveryType.Spot,
                        Type = "tolerances"
                    },
                    new Tolerance
                    {
                        RuleId = 26,
                        InternalType = "Campaign",
                        Description = "Booking Position",
                        Value = null,
                        Under = 0,
                        Over = 0,
                        Ignore = false,
                        ForceOverUnder = ForceOverUnder.None,
                        CampaignType = CampaignDeliveryType.Spot,
                        Type = "tolerances"
                    },
                    new Tolerance
                    {
                        RuleId = 27,
                        InternalType = "Campaign",
                        Description = "Centre/End",
                        Value = null,
                        Under = 0,
                        Over = 0,
                        Ignore = false,
                        ForceOverUnder = ForceOverUnder.None,
                        CampaignType = CampaignDeliveryType.Spot,
                        Type = "tolerances"
                    },
                    new Tolerance
                    {
                        RuleId = 20,
                        InternalType = "Sales Area",
                        Description = "Sales Area",
                        Value = null,
                        Under = 0,
                        Over = 5,
                        Ignore = false,
                        ForceOverUnder = ForceOverUnder.None,
                        CampaignType = CampaignDeliveryType.Spot,
                        Type = "tolerances"
                    },
                    new Tolerance
                    {
                        RuleId = 21,
                        InternalType = "Sales Area",
                        Description = "Spot Length",
                        Value = null,
                        Under = 0,
                        Over = 5,
                        Ignore = false,
                        ForceOverUnder = ForceOverUnder.None,
                        CampaignType = CampaignDeliveryType.Spot,
                        Type = "tolerances"
                    },
                    new Tolerance
                    {
                        RuleId = 22,
                        InternalType = "Sales Area",
                        Description = "Daypart",
                        Value = null,
                        Under = 0,
                        Over = 5,
                        Ignore = false,
                        ForceOverUnder = ForceOverUnder.None,
                        CampaignType = CampaignDeliveryType.Spot,
                        Type = "tolerances"
                    },
                    new Tolerance
                    {
                        RuleId = 23,
                        InternalType = "Sales Area",
                        Description = "Strike Weight",
                        Value = null,
                        Under = 0,
                        Over = 5,
                        Ignore = false,
                        ForceOverUnder = ForceOverUnder.None,
                        CampaignType = CampaignDeliveryType.Spot,
                        Type = "tolerances"
                    },
                    new Tolerance
                    {
                        RuleId = 24,
                        InternalType = "Sales Area",
                        Description = "Strike Wgt/Daypart",
                        Value = null,
                        Under = 0,
                        Over = 5,
                        Ignore = false,
                        ForceOverUnder = ForceOverUnder.None,
                        CampaignType = CampaignDeliveryType.Spot,
                        Type = "tolerances"
                    },
                    new Tolerance
                    {
                        RuleId = 25,
                        InternalType = "Sales Area",
                        Description = "Peak Daypart",
                        Value = null,
                        Under = 0,
                        Over = 0,
                        Ignore = false,
                        ForceOverUnder = ForceOverUnder.None,
                        CampaignType = CampaignDeliveryType.Spot,
                        Type = "tolerances"
                    },
                    new Tolerance
                    {
                        RuleId = 30,
                        InternalType = "Sales Area",
                        Description = "Daypart/Length",
                        Value = null,
                        Under = 0,
                        Over = 5,
                        Ignore = false,
                        ForceOverUnder = ForceOverUnder.None,
                        CampaignType = CampaignDeliveryType.Spot,
                        Type = "tolerances"
                    },
                    new Tolerance
                    {
                        RuleId = 31,
                        InternalType = "Sales Area",
                        Description = "Strike Weight/Length",
                        Value = null,
                        Under = 0,
                        Over = 0,
                        Ignore = true,
                        ForceOverUnder = ForceOverUnder.None,
                        CampaignType = CampaignDeliveryType.Spot,
                        Type = "tolerances"
                    },
                    new Tolerance
                    {
                        RuleId = 32,
                        InternalType = "Sales Area",
                        Description = "Strike Weight/Daypart/Length",
                        Value = null,
                        Under = 0,
                        Over = 0,
                        Ignore = true,
                        ForceOverUnder = ForceOverUnder.None,
                        CampaignType = CampaignDeliveryType.Spot,
                        Type = "tolerances"
                    },
                    new Tolerance
                    {
                        RuleId = 28,
                        InternalType = "Budget",
                        Description = "Campaign (Budget)",
                        Value = null,
                        Under = 0,
                        Over = 0,
                        Ignore = true,
                        ForceOverUnder = ForceOverUnder.None,
                        CampaignType = CampaignDeliveryType.Spot,
                        Type = "tolerances"
                    },
                    new Tolerance
                    {
                        RuleId = 29,
                        InternalType = "Budget",
                        Description = "Sales Area (Budget)",
                        Value = null,
                        Under = 0,
                        Over = 0,
                        Ignore = true,
                        ForceOverUnder = ForceOverUnder.None,
                        CampaignType = CampaignDeliveryType.Spot,
                        Type = "tolerances"
                    }
                },
                Rules = new List<PassRule>
                {
                    new PassRule
                    {
                        RuleId = 1,
                        InternalType = "Slotting Controls",
                        Description = "Max Spots Per Day",
                        Ignore = false,
                        Value = "15",
                        PeakValue = null,
                        Type = "rules",
                        CampaignType = CampaignDeliveryType.Spot
                    },
                    new PassRule
                    {
                        RuleId = 2,
                        InternalType = "Slotting Controls",
                        Description = "Max Spots Per Hour",
                        Ignore = false,
                        Value = "2",
                        PeakValue = null,
                        Type = "rules",
                        CampaignType = CampaignDeliveryType.Spot
                    },
                    new PassRule
                    {
                        RuleId = 3,
                        InternalType = "Slotting Controls",
                        Description = "Max Spots Per 2 Hours",
                        Ignore = false,
                        Value = "4",
                        PeakValue = null,
                        Type = "rules",
                        CampaignType = CampaignDeliveryType.Spot
                    },
                    new PassRule
                    {
                        RuleId = 4,
                        InternalType = "Slotting Controls",
                        Description = "Min Breaks Between Spots",
                        Ignore = false,
                        Value = "2",
                        PeakValue = null,
                        Type = "rules",
                        CampaignType = CampaignDeliveryType.Spot
                    },
                    new PassRule
                    {
                        RuleId = 5,
                        InternalType = "Slotting Controls",
                        Description = "Min Hours Between Spots",
                        Ignore = false,
                        Value = "0",
                        PeakValue = null,
                        Type = "rules",
                        CampaignType = CampaignDeliveryType.Spot
                    },
                    new PassRule
                    {
                        RuleId = 6,
                        InternalType = "Slotting Controls",
                        Description = "Max Spots Per Programme/Day",
                        Ignore = false,
                        Value = "2",
                        PeakValue = null,
                        Type = "rules",
                        CampaignType = CampaignDeliveryType.Spot
                    },
                    new PassRule
                    {
                        RuleId = 7,
                        InternalType = "Slotting Controls",
                        Description = "Max Spots Per Programme/Week",
                        Ignore = false,
                        Value = "5",
                        PeakValue = null,
                        Type = "rules",
                        CampaignType = CampaignDeliveryType.Spot
                    },
                    new PassRule
                    {
                        RuleId = 17,
                        InternalType = "Slotting Controls",
                        Description = "Max Spots per Prog/100 rtgs",
                        Ignore = false,
                        Value = "0",
                        PeakValue = null,
                        Type = "rules",
                        CampaignType = CampaignDeliveryType.Spot
                    },
                    new PassRule
                    {
                        RuleId = 18,
                        InternalType = "Slotting Controls",
                        Description = "Max Zero Ratings",
                        Ignore = false,
                        Value = "1",
                        PeakValue = null,
                        Type = "rules",
                        CampaignType = CampaignDeliveryType.Spot
                    },
                    new PassRule
                    {
                        RuleId = 20,
                        InternalType = "Slotting Controls",
                        Description = "Min Weeks Between Programmes",
                        Ignore = false,
                        Value = "0",
                        PeakValue = null,
                        Type = "rules",
                        CampaignType = CampaignDeliveryType.Spot
                    },
                    new PassRule
                    {
                        RuleId = 21,
                        InternalType = "Slotting Controls",
                        Description = "Max Ratings for Spot Campaigns",
                        Ignore = false,
                        Value = "0",
                        PeakValue = null,
                        Type = "rules",
                        CampaignType = CampaignDeliveryType.Spot
                    },
                    new PassRule
                    {
                        RuleId = 23,
                        InternalType = "Slotting Controls",
                        Description = "Minimum Break Availability",
                        Ignore = false,
                        Value = "5",
                        PeakValue = null,
                        Type = "rules",
                        CampaignType = CampaignDeliveryType.Spot
                    },
                    new PassRule
                    {
                        RuleId = 24,
                        InternalType = "Slotting Controls",
                        Description = "Max Spots Per Prog/Time",
                        Ignore = false,
                        Value = "2",
                        PeakValue = null,
                        Type = "rules",
                        CampaignType = CampaignDeliveryType.Spot
                    },
                    new PassRule
                    {
                        RuleId = 25,
                        InternalType = "Slotting Controls",
                        Description = "Min Days Between Prog/Time",
                        Ignore = false,
                        Value = "0",
                        PeakValue = null,
                        Type = "rules",
                        CampaignType = CampaignDeliveryType.Spot
                    },
                    new PassRule
                    {
                        RuleId = 26,
                        InternalType = "Slotting Controls",
                        Description = "Min Weeks Between Prog/Time",
                        Ignore = false,
                        Value = "0",
                        PeakValue = null,
                        Type = "rules",
                        CampaignType = CampaignDeliveryType.Spot
                    },
                    new PassRule
                    {
                        RuleId = 27,
                        InternalType = "Slotting Controls",
                        Description = "Max Ratings for Rating Campaigns",
                        Ignore = false,
                        Value = "9999999",
                        PeakValue = null,
                        Type = "rules",
                        CampaignType = CampaignDeliveryType.Rating
                    },
                    new PassRule
                    {
                        RuleId = 28,
                        InternalType = "Slotting Controls",
                        Description = "Max Spots Per Day",
                        Ignore = false,
                        Value = "15",
                        PeakValue = null,
                        Type = "rules",
                        CampaignType = CampaignDeliveryType.Rating
                    },
                    new PassRule
                    {
                        RuleId = 29,
                        InternalType = "Slotting Controls",
                        Description = "Max Spots Per Hour",
                        Ignore = false,
                        Value = "2",
                        PeakValue = null,
                        Type = "rules",
                        CampaignType = CampaignDeliveryType.Rating
                    },
                    new PassRule
                    {
                        RuleId = 30,
                        InternalType = "Slotting Controls",
                        Description = "Max Spots Per 2 Hours",
                        Ignore = false,
                        Value = "4",
                        PeakValue = null,
                        Type = "rules",
                        CampaignType = CampaignDeliveryType.Rating
                    },
                    new PassRule
                    {
                        RuleId = 31,
                        InternalType = "Slotting Controls",
                        Description = "Min Breaks Between Spots",
                        Ignore = false,
                        Value = "2",
                        PeakValue = null,
                        Type = "rules",
                        CampaignType = CampaignDeliveryType.Rating
                    },
                    new PassRule
                    {
                        RuleId = 32,
                        InternalType = "Slotting Controls",
                        Description = "Min Hours Between Spots",
                        Ignore = false,
                        Value = "0",
                        PeakValue = null,
                        Type = "rules",
                        CampaignType = CampaignDeliveryType.Rating
                    },
                    new PassRule
                    {
                        RuleId = 33,
                        InternalType = "Slotting Controls",
                        Description = "Max Spots Per Programme/Day",
                        Ignore = false,
                        Value = "2",
                        PeakValue = null,
                        Type = "rules",
                        CampaignType = CampaignDeliveryType.Rating
                    },
                    new PassRule
                    {
                        RuleId = 34,
                        InternalType = "Slotting Controls",
                        Description = "Max Spots Per Programme/Week",
                        Ignore = false,
                        Value = "5",
                        PeakValue = null,
                        Type = "rules",
                        CampaignType = CampaignDeliveryType.Rating
                    },
                    new PassRule
                    {
                        RuleId = 44,
                        InternalType = "Slotting Controls",
                        Description = "Max Spots per Prog/100 rtgs",
                        Ignore = false,
                        Value = "0",
                        PeakValue = null,
                        Type = "rules",
                        CampaignType = CampaignDeliveryType.Rating
                    },
                    new PassRule
                    {
                        RuleId = 45,
                        InternalType = "Slotting Controls",
                        Description = "Max Zero Ratings",
                        Ignore = false,
                        Value = "1",
                        PeakValue = null,
                        Type = "rules",
                        CampaignType = CampaignDeliveryType.Rating
                    },
                    new PassRule
                    {
                        RuleId = 47,
                        InternalType = "Slotting Controls",
                        Description = "Min Weeks Between Programmes",
                        Ignore = false,
                        Value = "0",
                        PeakValue = null,
                        Type = "rules",
                        CampaignType = CampaignDeliveryType.Rating
                    },
                    new PassRule
                    {
                        RuleId = 50,
                        InternalType = "Slotting Controls",
                        Description = "Minimum Break Availability",
                        Ignore = false,
                        Value = "5",
                        PeakValue = null,
                        Type = "rules",
                        CampaignType = CampaignDeliveryType.Rating
                    },
                    new PassRule
                    {
                        RuleId = 51,
                        InternalType = "Slotting Controls",
                        Description = "Max Spots Per Prog/Time",
                        Ignore = false,
                        Value = "2",
                        PeakValue = null,
                        Type = "rules",
                        CampaignType = CampaignDeliveryType.Rating
                    },
                    new PassRule
                    {
                        RuleId = 52,
                        InternalType = "Slotting Controls",
                        Description = "Min Days Between Prog/Time",
                        Ignore = false,
                        Value = "0",
                        PeakValue = null,
                        Type = "rules",
                        CampaignType = CampaignDeliveryType.Rating
                    },
                    new PassRule
                    {
                        RuleId = 53,
                        InternalType = "Slotting Controls",
                        Description = "Min Weeks Between Prog/Time",
                        Ignore = false,
                        Value = "0",
                        PeakValue = null,
                        Type = "rules",
                        CampaignType = CampaignDeliveryType.Rating
                    },
                },
                ProgrammeRepetitions = new List<ProgrammeRepetition>
                {
                    new ProgrammeRepetition
                    {
                        Minutes = 60,
                        Factor = 2,
                        PeakFactor = null
                    },
                    new ProgrammeRepetition
                    {
                        Minutes = 120,
                        Factor = 4,
                        PeakFactor = null
                    },
                    new ProgrammeRepetition
                    {
                        Minutes = 180,
                        Factor = 5,
                        PeakFactor = null
                    },
                },
                BreakExclusions = new List<BreakExclusion>
                {
                    new BreakExclusion
                    {
                        SalesArea = salesAreas[_randomizer.Next(salesAreas.Length)].Name,
                        StartDate = utcDate.Date.AddDays(-5),
                        EndDate = utcDate.Date.AddDays(-5),
                        StartTime = TimeSpan.FromHours(6),
                        EndTime = TimeSpan.FromHours(5.75),
                        SelectableDays = new List<DayOfWeek>
                        {
                            DayOfWeek.Sunday
                        }
                    }
                },
                SlottingLimits = new List<SlottingLimit>
                {
                    new SlottingLimit
                    {
                        Demographs = "15",
                        MinimumEfficiency = 1,
                        MaximumEfficiency = 99999,
                        BandingTolerance = 5
                    }
                },
                PassSalesAreaPriorities = new PassSalesAreaPriority
                {
                    SalesAreaPriorities = salesAreas.Select(sa => new SalesAreaPriority
                    {
                        SalesArea = sa.Name,
                        Priority = SalesAreaPriorityType.Priority3
                    }).ToList(),
                    StartDate = utcDate.Date,
                    EndDate = utcDate.Date.AddDays(5),
                    StartTime = new TimeSpan(0, 6, 0, 0),
                    EndTime = new TimeSpan(0, 5, 59, 59),
                    DaysOfWeek = "1111111"
                },
            };
        }

        private Scenario CreateScenario(Campaign[] campaigns, Pass pass, Product product, Clash clash)
        {
            return new Scenario
            {
                Name = $"{nameof(Scenario)}{nameof(Scenario.Name)}{Guid.NewGuid()}",
                CustomId = _identityGeneratorResolver.Resolve<IScenarioRepository>()
                    .GetIdentities<ScenarioNoIdentity>(1).First().Id,
                IsLibraried = false,
                Passes = new List<PassReference>
                {
                    new PassReference
                    {
                        Id = pass.Id
                    }
                },
                CampaignPassPriorities = campaigns.Select(c => new CampaignPassPriority
                {
                    Campaign = new CompactCampaign
                    {
                        Uid = c.Id,
                        CustomId = c.CustomId,
                        Status = new Func<string>(() =>
                        {
                            switch (c.Status)
                            {
                                case "A":
                                    return "Active";
                                case "C":
                                    return "Cancelled";
                                default:
                                    return "Unknown";
                            }
                        })(),
                        Name = c.Name,
                        ExternalId = c.ExternalId,
                        CampaignGroup = c.CampaignGroup,
                        StartDateTime = c.StartDateTime,
                        EndDateTime = c.EndDateTime,
                        ProductExternalRef = product.Externalidentifier,
                        ProductName = product.Name,
                        AdvertiserName = product.AdvertiserName,
                        AgencyName = product.AgencyName,
                        BusinessType = c.BusinessType,
                        DeliveryType = c.DeliveryType,
                        Demographic = c.DemoGraphic,
                        RevenueBudget = c.RevenueBudget,
                        TargetRatings = c.TargetRatings,
                        ActualRatings = c.ActualRatings,
                        IsPercentage = c.IsPercentage,
                        IncludeOptimisation = c.IncludeOptimisation,
                        TargetZeroRatedBreaks = c.TargetZeroRatedBreaks,
                        InefficientSpotRemoval = c.InefficientSpotRemoval,
                        IncludeRightSizer = c.RightSizerLevel.HasValue
                            ? (IncludeRightSizer)c.RightSizerLevel.Value
                            : IncludeRightSizer.No,
                        DefaultCampaignPassPriority = c.CampaignPassPriority,
                        ClashCode = clash.Externalref,
                        ClashDescription = clash.Description
                    },
                    PassPriorities = new List<PassPriority>
                    {
                        new PassPriority
                        {
                            PassId = pass.Id,
                            PassName = pass.Name,
                            Priority = c.CampaignPassPriority
                        }
                    }
                }).ToList()
            };
        }

        private Run CreateRun(Scenario scenario, SalesArea[] salesAreas)
        {
            var utcDate = _clock.GetCurrentInstant().ToDateTimeUtc();
            return new Run
            {
                SalesAreaPriorities = salesAreas.Select(sa => new SalesAreaPriority
                {
                    SalesArea = sa.Name,
                    Priority = SalesAreaPriorityType.Priority3
                }).ToList(),
                CustomId = _identityGeneratorResolver.Resolve<IRunRepository>()
                    .GetIdentities<RunNoIdentity>(1).First().Id,
                Description = $"{nameof(Run.Description)}{Guid.NewGuid()}",
                CreatedDateTime = utcDate,
                StartDate = utcDate.Date.AddDays(4),
                StartTime = TimeSpan.FromHours(6),
                EndDate = utcDate.Date.AddDays(9),
                EndTime = new TimeSpan(0, 5, 59, 59),
                LastModifiedDateTime = utcDate,
                ExecuteStartedDateTime = utcDate,
                IsLocked = false,
                Real = true,
                Smooth = true,
                SmoothDateRange = new DateRange(utcDate.Date.AddDays(4), utcDate.Date.AddDays(9)),
                ISR = false,
                ISRDateRange = new DateRange(utcDate.Date.AddDays(4), utcDate.Date.AddDays(4)),
                Optimisation = true,
                OptimisationDateRange = new DateRange(utcDate.Date.AddDays(4), utcDate.Date.AddDays(9)),
                RightSizer = false,
                RightSizerDateRange = new DateRange(utcDate.Date.AddDays(4), utcDate.Date.AddDays(4)),
                SpreadProgramming = false,
                SkipLockedBreaks = false,
                IgnorePremiumCategoryBreaks = false,
                ExcludeBankHolidays = false,
                ExcludeSchoolHolidays = false,
                IgnoreZeroPercentageSplit = false,
                BookTargetArea = false,
                Objectives = "Test run result",
                Author = new AuthorModel
                {
                    Id = 1001,
                    Name = "Chris"
                },
                EfficiencyPeriod = EfficiencyCalculationPeriod.RunPeriod,
                NumberOfWeeks = 2,
                Campaigns = new List<CampaignReference>(),
                CampaignsProcessesSettings = new List<CampaignRunProcessesSettings>(),
                Scenarios = new List<RunScenario>
                {
                    new RunScenario
                    {
                        Id = scenario.Id,
                        StartedDateTime = utcDate,
                        CompletedDateTime = utcDate.AddSeconds(1),
                        Progress = "DRAFT",
                        Status = ScenarioStatuses.CompletedSuccess
                    }
                },
                Manual = false
            };
        }

        private ScenarioResult CreateScenarioResult(Scenario scenario)
        {
            return new ScenarioResult
            {
                Id = scenario.Id,
                TimeCompleted = _clock.GetCurrentInstant().ToDateTimeUtc().AddSeconds(1),
                Metrics = new List<KPI>
                {
                    new KPI
                    {
                        Name = "percent95to105",
                        Displayformat = "largenumber",
                        Value = 3
                    },
                    new KPI
                    {
                        Name = "percentgreater105",
                        Displayformat = "largenumber",
                        Value = 0
                    },
                    new KPI
                    {
                        Name = "percent75to95",
                        Displayformat = "largenumber",
                        Value = 1
                    },
                    new KPI
                    {
                        Name = "noOfCampaigns",
                        Displayformat = "largenumber",
                        Value = 10
                    },
                    new KPI
                    {
                        Name = "percentbelow75",
                        Displayformat = "largenumber",
                        Value = 5
                    },
                    new KPI
                    {
                        Name = "averageEfficiency",
                        Displayformat = "largenumber",
                        Value = 194.8
                    },
                    new KPI
                    {
                        Name = "averagecancelEfficiency",
                        Displayformat = "largenumber",
                        Value = 0
                    },
                    new KPI
                    {
                        Name = "totalSpotsBooked",
                        Displayformat = "largenumber",
                        Value = 744
                    },
                    new KPI
                    {
                        Name = "remainaudience",
                        Displayformat = "largenumber",
                        Value = 995697948
                    },
                    new KPI
                    {
                        Name = "remainingAvailability",
                        Displayformat = "largenumber",
                        Value = 1585125
                    },
                    new KPI
                    {
                        Name = "standardAverageCompletion",
                        Displayformat = "percentage",
                        Value = 95.26
                    },
                    new KPI
                    {
                        Name = "weightedAverageCompletion",
                        Displayformat = "percentage",
                        Value = 96.83
                    }
                }
            };
        }

        private Failures CreateFailures(Scenario scenario, Campaign[] campaigns)
        {
            return new Failures
            {
                Id = scenario.Id,
                Items = campaigns.Select(c => new Failure
                {
                    Campaign = c.CustomId,
                    CampaignName = c.Name,
                    ExternalId = c.ExternalId,
                    Type = 3,
                    Failures = _randomizer.Next(10) + 1,
                    SalesAreaName = c.SalesAreaCampaignTarget[_randomizer.Next(c.SalesAreaCampaignTarget.Count)]
                        .SalesAreaGroup.GroupName

                }).ToList()
            };
        }

        private Programme[] CreateProgrammes(SalesArea[] salesAreas)
        {
            var utcDate = _clock.GetCurrentInstant().ToDateTimeUtc();
            return salesAreas.Select((sa, idx) => new Programme
            {
                PrgtNo = idx + 1,
                SalesArea = sa.Name,
                StartDateTime = utcDate.Date.AddMonths(6),
                Duration = Duration.FromHours(3),
                ExternalReference = $"{nameof(Programme.ExternalReference)}{Guid.NewGuid()}",
                ProgrammeName = $"{nameof(Programme.ProgrammeName)}{Guid.NewGuid()}",
                ProgrammeCategories = new List<string>(),
                Classification = "M",
                LiveBroadcast = false

            }).ToArray();
        }

        private Break[] CreateBreaks(SalesArea[] salesAreas)
        {
            var utcDate = _clock.GetCurrentInstant().ToDateTimeUtc();
            return salesAreas.Select(sa => new Break
            {
                Id = Guid.NewGuid(),
                CustomId = _randomizer.Next(),
                ScheduledDate = utcDate.AddDays(4),
                SalesArea = sa.Name,
                BreakType = "BASE",
                Duration = Duration.FromSeconds(120),
                Avail = Duration.FromSeconds(90),
                OptimizerAvail = Duration.FromSeconds(90),
                Optimize = true,
                ExternalBreakRef = $"{nameof(Break.ExternalBreakRef)}{Guid.NewGuid()}",
                PositionInProg = BreakPosition.C

            }).ToArray();
        }

        private Recommendation[] CreateRecommendations(Campaign[] campaigns, Scenario scenario, Pass pass,
            Programme[] programmes, Break[] breaks)
        {
            var utcDate = _clock.GetCurrentInstant().ToDateTimeUtc();
            return campaigns.SelectMany((c, cidx) =>
            {
                return c.SalesAreaCampaignTarget.Select((sa, sidx) => new Recommendation
                {
                    ScenarioId = scenario.Id,
                    ExternalSpotRef = null,
                    ExternalCampaignNumber = c.ExternalId,
                    SpotLength = Duration.FromSeconds(15),
                    Product = c.Product,
                    Demographic = c.DemoGraphic,
                    StartDateTime = utcDate.AddDays(4).AddSeconds((600 * cidx) + (sidx * 15)),
                    SalesArea = sa.SalesAreaGroup.GroupName,
                    ExternalProgrammeReference = programmes
                        .FirstOrDefault(p => p.SalesArea == sa.SalesAreaGroup.GroupName)?.ExternalReference,
                    ProgrammeName = programmes
                        .FirstOrDefault(p => p.SalesArea == sa.SalesAreaGroup.GroupName)?.ProgrammeName,
                    BreakType = "BASE",
                    SpotRating = _randomizer.Next(19000) + 1000,
                    SpotEfficiency = (_randomizer.Next(2000) + 1000) / 10.0,
                    Action = "B",
                    Processor = "autobook",
                    ProcessorDateTime = utcDate,
                    GroupCode = sa.SalesAreaGroup.GroupName,
                    ClientPicked = false,
                    ExternalBreakNo = breaks.FirstOrDefault(b => b.SalesArea == sa.SalesAreaGroup.GroupName)
                        ?.ExternalBreakRef,
                    PassName = pass.Name,
                    OptimiserPassSequenceNumber = sidx
                });
            }).ToArray();
        }

        private IDictionary<string, long> CreateResultFiles(TestEnvironmentOutputFilesInfo outputFilesInfo, Guid scenarioId)
        {
            return outputFilesInfo.FileNames.Select(fileName =>
            {
                _resultsFileRepository.Insert(scenarioId, fileName, outputFilesInfo.LocalFolder);
                return fileName;
            }).ToDictionary(k => $"file:{k}", v => new FileInfo(Path.Combine(outputFilesInfo.LocalFolder, v)).Length);
        }

        #endregion
    }
}
