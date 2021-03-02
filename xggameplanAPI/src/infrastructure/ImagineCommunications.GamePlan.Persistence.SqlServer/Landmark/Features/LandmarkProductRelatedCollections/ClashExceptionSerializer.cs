using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ClashExceptions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Views.Tenant;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using NodaTime;
using xggameplan.AuditEvents;
using xggameplan.common.Extensions;
using xggameplan.core.Services.OptimiserInputFilesSerialisers;
using xggameplan.core.Services.OptimiserInputFilesSerialisers.Interfaces;
using xggameplan.Extensions;
using xggameplan.Model.AutoGen;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Landmark.Features.LandmarkProductRelatedCollections
{
    /// <summary>
    /// Serializes clash exception into xml file taking into account
    /// Landmark Product related data by its active periods.
    /// </summary>
    public class ClashExceptionSerializer : SerializerBase, IClashExceptionSerializer
    {
        private const string AgDateFormat = "yyyyMMdd";
        private const string AgTimeFormat = "hhmmss";
        private const string DefaultAgMinTime = "0";
        private const string DefaultAgMaxTime = "995959";

        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IClock _clock;

        /// <summary>Initializes a new instance of the <see cref="ClashExceptionSerializer" /> class.</summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="auditEventRepository">The audit event repository.</param>
        /// <param name="clock">The clock.</param>
        public ClashExceptionSerializer(
            ISqlServerTenantDbContext dbContext,
            IAuditEventRepository auditEventRepository,
            IClock clock) : base(auditEventRepository)
        {
            _dbContext = dbContext;
            _clock = clock;
        }

        /// <summary>Gets the filename.</summary>
        /// <value>The filename.</value>
        public string Filename => "v_clsh_exc_list.xml";

        /// <summary>Serializes clash exceptions into the specified folder name.</summary>
        /// <param name="folderName">Name of the folder.</param>
        /// <param name="run">The run.</param>
        /// <param name="clashes">The clashes.</param>
        /// <param name="filteredProducts">List of products filtered by included campaign list.</param>
        public void Serialize(string folderName, Run run, IReadOnlyCollection<Clash> clashes, IReadOnlyCollection<Product> filteredProducts)
        {
            var clashExceptions =
                _dbContext.Query<ClashException>().Include(x => x.ClashExceptionsTimeAndDows).ToArray();

            var productLinks = _dbContext.Specific
                                   .StoredProcedure<ClashExceptionProductLinkForOptimizer>(
                                       new MySqlParameter("@EndDate", run.EndDate))
                                   .AsEnumerable()?
                                   .GroupBy(key => key.Id)
                                   .ToDictionary(k => k.Key,
                                       v => v.AsEnumerable())
                               ?? new Dictionary<int, IEnumerable<ClashExceptionProductLinkForOptimizer>>();

            if (clashExceptions.Length == 0)
            {
                return;
            }

            RaiseInfo(
                $"Started populating {Filename}. Total clash exceptions - {clashExceptions.Length}, Current Time - {_clock.GetCurrentInstant().ToDateTimeUtc()}");

            var agClashExceptions = ConvertToAgClashException(clashExceptions, productLinks, run);

            new AgClashExceptionsSerialisation().MapFrom(agClashExceptions)
                .Serialize(Path.Combine(folderName, Filename));

            RaiseInfo(
                $"Finished populating {Filename}. Total clash exceptions - {clashExceptions.Length}, Current Time - {_clock.GetCurrentInstant().ToDateTimeUtc()}");
        }

        /// <summary>Converts to ag clash exception.</summary>
        /// <param name="clashExceptions">The clash exceptions.</param>
        /// <param name="productLinks">The product links.</param>
        /// <param name="run">The run.</param>
        /// <returns></returns>
        private List<AgClashException> ConvertToAgClashException(
            IReadOnlyCollection<ClashException> clashExceptions,
            IDictionary<int, IEnumerable<ClashExceptionProductLinkForOptimizer>> productLinks,
            Run run)
        {
            var runEndDate = run.EndDate.AddYears(10).ToString(AgDateFormat);

            return clashExceptions.SelectMany(ce =>
            {
                var productLinkExists = productLinks.TryGetValue(ce.Id, out var productLink);
                if (!productLinkExists)
                {
                    return Enumerable.Empty<AgClashException>();
                }

                return
                    from dow in ce.ClashExceptionsTimeAndDows
                    from dayRange in dow.DaysOfWeek.GetDayRangeFromDayCode()
                    from link in productLink
                    select new AgClashException
                    {
                        FromClashCode = ce.FromType == ClashExceptionType.Clash ? link.FromValue : string.Empty,
                        ToClashCode = ce.ToType == ClashExceptionType.Clash ? link.ToValue : string.Empty,
                        FromProductCode = ce.FromType != ClashExceptionType.Clash
                            ? Convert.ToInt32(link.FromValue)
                            : 0,
                        ToProductCode = ce.ToType != ClashExceptionType.Clash
                            ? Convert.ToInt32(link.ToValue)
                            : 0,
                        DefinedClash = 1,
                        EndDate = ce.EndDate.HasValue
                            ? ce.EndDate.Value.ToString(AgDateFormat)
                            : runEndDate,
                        StartDate = ce.StartDate.ToString(AgDateFormat),
                        IncludeExcludeFlag = ce.IncludeOrExclude.GetDescription(),
                        StartTime = dow.StartTime?.ToString(AgTimeFormat) ?? DefaultAgMinTime,
                        EndTime = dow.EndTime?.ToString(AgTimeFormat) ?? DefaultAgMaxTime,
                        StartDayNo = dayRange.Item1,
                        EndDayNo = dayRange.Item2
                    };
            }).ToList();
        }
    }
}
