using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using NodaTime;
using xggameplan.AuditEvents;
using xggameplan.core.Services.OptimiserInputFilesSerialisers.Interfaces;
using xggameplan.Extensions;
using xggameplan.Model.AutoGen;

namespace xggameplan.core.Services.OptimiserInputFilesSerialisers
{
    public class ProgTxDetailSerializer : SerializerBase, IProgTxDetailSerializer
    {
        private const string ShortDateFormat = "yyyyMMdd";
        private const string TimeFormat = "HHmmss";
        private const string Filename = "v_rb_prgt_list.xml";

        private readonly IClock _clock;

        protected virtual DateTime AdjustScheduledEndTime(DateTime value)
        {
            return value;
        }

        public ProgTxDetailSerializer(IAuditEventRepository auditEventRepository, IClock clock)
            : base(auditEventRepository)
        {
            _clock = clock;
        }

        public void Serialize(
            string folderName,
            IReadOnlyCollection<Programme> programs,
            IReadOnlyCollection<ProgrammeDictionary> programmeDictionaries,
            IReadOnlyCollection<ProgrammeCategoryHierarchy> programmeCategories,
            IReadOnlyCollection<SalesArea> salesAreas,
            IAutoBookDefaultParameters autoBookDefaultParameters)
        {
            RaiseInfo(
                $"Started populating {Filename}. Total programmes - {programs.Count}, Current Time - {_clock.GetCurrentInstant().ToDateTimeUtc()}");

            List<AgProgTxDetail> data = programs.SelectMany(pgm =>
            {
                int programmeNo = programmeDictionaries
                    .FirstOrDefault(p =>
                        p.ExternalReference.Equals(pgm.ExternalReference, StringComparison.OrdinalIgnoreCase))?.Id ?? 0;
                int salesAreaNo = salesAreas.FirstOrDefault(s => s.Name.Equals(pgm.SalesArea))?.CustomId ?? 0;

                return pgm.ProgrammeCategories.DefaultIfEmpty().Select(c =>
                {
                    return new AgProgTxDetail
                    {
                        ProgrammeNo = programmeNo,
                        EpisodeNo = pgm.Episode?.Number ?? autoBookDefaultParameters.AgProgTxDetail.EpisodeNo,
                        SalesAreaNo = salesAreaNo,
                        TregNo = salesAreaNo,
                        TxDate = pgm.StartDateTime.ToString(ShortDateFormat),
                        ScheduledStartTime = pgm.StartDateTime.ToString(TimeFormat),
                        ScheduledEndTime =
                            AdjustScheduledEndTime(pgm.StartDateTime.AddTicks(pgm.Duration.BclCompatibleTicks))
                                .ToString(TimeFormat),
                        ProgCategoryNo = c != null
                            ? programmeCategories.FirstOrDefault(pc => pc.Name
                                .Equals(c, StringComparison.OrdinalIgnoreCase))?.Id ?? 0
                            : 0,
                        ClassCode = pgm.Classification,
                        LiveBroadcast = pgm.LiveBroadcast ? "Y" : "N"
                    };
                });
            }).Distinct().ToList();

            new AgProgTxDetailsSerialization().MapFrom(data).Serialize(Path.Combine(folderName, Filename));

            RaiseInfo(
                $"Finished populating {Filename}. Total programmes - {programs.Count}, Current Time - {_clock.GetCurrentInstant().ToDateTimeUtc()}");
        }
    }
}
