using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Helpers;
using ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using NodaTime;
using xggameplan.core.Profile;
using xggameplan.CSVImporter;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class RecommendationProfile : AutoMapper.Profile
    {
        private const string AutoProcessor = "autobook";
        private const string IsrProcessor = "isr";
        private const string RsProcessor = "rzr";

        public RecommendationProfile()
        {
            CreateMap<Recommendation, RecommendationModel>()
                .ForMember(x => x.GroupCode, e => e.MapFrom(a => GetIntOrDefault(a.GroupCode, 0)))
                .ForMember(x => x.Product, e => e.MapFrom(a => GetIntOrDefault(a.Product, 0)))
                .ForMember(x => x.Demographic, e => e.MapFrom(a => GetIntOrDefault(a.Demographic, 0)));

            CreateMap<Recommendation, model.External.RecommendationReducedModel>();

            CreateMap<RecommendationSimple, RecommendationSimpleModel>();

            CreateMap<Tuple<SpotReqmImport, List<ProgrammeDictionary>, IEnumerable<string>>, Recommendation>()
                .ForMember(x => x.BreakBookingPosition, expression => expression.MapFrom(spotImport => 0))
                .ForMember(x => x.StartDateTime, expression => expression.MapFrom(spotImport => GetStartDateTime(spotImport.Item1)))
                .ForMember(x => x.EndDateTime, expression => expression.MapFrom(spotImport => GetEndDateTime(spotImport.Item1, Duration.FromSeconds(spotImport.Item1.sslg_length))))
                .ForMember(x => x.BreakType, expression => expression.MapFrom(spotImport => GetBreakType(spotImport.Item1.btyp_code, spotImport.Item3)))
                //.ForMember(x => x.Demographic, expression => expression.MapFrom(spotImport => spotImport.Item1.demo_no.ToString()))
                .ForMember(x => x.ExternalCampaignNumber, expression => expression.MapFrom(spotImport => spotImport.Item1.camp_no.ToString()))
                .ForMember(x => x.Product, expression => expression.MapFrom(spotImport => spotImport.Item1.prod_code.ToString()))
                .ForMember(x => x.ExternalProgrammeReference, expression => expression.MapFrom(spotImport => GetExternalProgrammeReference(spotImport.Item1.prog_no, spotImport.Item2)))
                .ForMember(x => x.ProgrammeName, expression => expression.MapFrom(spotImport => GetProgrammeName(spotImport.Item1.prog_no, spotImport.Item2)))
                // .ForMember(x => x.SalesArea, expression => expression.MapFrom(spotImport => spotImport.spot_sare_no))
                .ForMember(x => x.SpotEfficiency, expression => expression.MapFrom(spotImport => spotImport.Item1.efficiency))
                .ForMember(x => x.SpotLength, expression => expression.MapFrom(spotImport => Duration.FromSeconds(spotImport.Item1.sslg_length)))
                //.ForMember(x => x.ExternalSpotRef, expression => expression.MapFrom(spotImport => ""))   // Blank, was spotImport.Item1.spot_no.ToString()
                .ForMember(x => x.SpotRating, expression => expression.MapFrom(spotImport => spotImport.Item1.rating))
                .ForMember(x => x.Action, expression => expression.MapFrom(spotImport => GetAction(spotImport.Item1)))
                .ForMember(x => x.RatingPoints, expression => expression.MapFrom(spotImport => spotImport.Item1.tarps))
                .ForMember(x => x.Processor, expression => expression.MapFrom(spotImport => GetProcessor(spotImport.Item1)))
                .ForMember(x => x.ProcessorDateTime, opts => opts.Ignore())     // Manually
                                                                                //.ForMember(x => x.GroupCode, expression => expression.MapFrom(spotImport => spotImport.Item1.spot_sare_no))
                                                                                // ForMember(x => x.ClientPicked, expression => expression.MapFrom(spotImport => true))           // Correct? (SPOTS.OUT has client_picked)
                                                                                //.ForMember(x => x.MultipartSpot, expression => expression.MapFrom(spotImport => spotImport.Item1.mpart_spot_ind))
                                                                                //.ForMember(x => x.MultipartSpotPosition, expression => expression.MapFrom(spotImport => spotImport.Item1.bkpo_posn_reqm))
                                                                                //.ForMember(x => x.MultipartSpotRef, expression => expression.MapFrom(spotImport => ""))
                .ForMember(x => x.RequestedPositionInBreak, expression => expression.MapFrom(spotImport => GetRequestedPositionInBreak(spotImport.Item1.bkpo_posn_reqm.GetValueOrDefault(0))))
                .ForMember(x => x.ActualPositionInBreak, expression => expression.MapFrom(spotImport => GetRequestedPositionInBreak(spotImport.Item1.bkpo_posn_reqm.GetValueOrDefault(0))))
                //.ForMember(x => x.ExternalBreakNo, expression => expression.MapFrom(spotImport => ""))      // Correct? (SPOTS.OUT has break_no)
                .ForMember(x => x.Filler, expression => expression.MapFrom(spotImport => false))
                .ForMember(x => x.Sponsored, expression => expression.MapFrom(spotImport => false))     // Spots.Sponsored
                .ForMember(x => x.Preemptable, expression => expression.MapFrom(spotImport => false))
                .ForMember(x => x.Preemptlevel, expression => expression.MapFrom(spotImport => 0));

            CreateMap<Tuple<SpotImport, List<ProgrammeDictionary>>, Recommendation>()
               .ForMember(x => x.BreakBookingPosition, expression => expression.MapFrom(spotImport => 0))
               .ForMember(x => x.StartDateTime, expression => expression.MapFrom(spotImport => GetStartDateTime(spotImport.Item1)))
               .ForMember(x => x.EndDateTime, expression => expression.MapFrom(spotImport => DateTime.MinValue))
               //.ForMember(x => x.BreakType, expression => expression.MapFrom(spotImport => GetBreakType(spotImport.Item1.btyp_code)))
               .ForMember(x => x.BreakType, expression => expression.MapFrom(spotImport => "BASE"))
               //.ForMember(x => x.Demographic, expression => expression.MapFrom(spotImport => spotImport.Item1.demo_no.ToString()))
               .ForMember(x => x.ExternalCampaignNumber, expression => expression.MapFrom(spotImport => spotImport.Item1.camp_no.ToString()))
               .ForMember(x => x.Product, expression => expression.MapFrom(spotImport => spotImport.Item1.prod_code.ToString()))
               //.ForMember(x => x.ExternalProgrammeReference, expression => expression.MapFrom(spotImport => GetExternalProgrammeReference(spotImport.Item1.prog_no, spotImport.Item2)))
               .ForMember(x => x.ExternalProgrammeReference, expression => expression.MapFrom(spotImport => (string)null))
               .ForMember(x => x.ProgrammeName, expression => expression.MapFrom(spotImport => (string)null))
               // .ForMember(x => x.SalesArea, expression => expression.MapFrom(spotImport => spotImport.spot_sare_no))
               .ForMember(x => x.SpotEfficiency, expression => expression.MapFrom(spotImport => spotImport.Item1.efficiency))
               .ForMember(x => x.SpotLength, expression => expression.MapFrom(spotImport => Duration.FromSeconds(spotImport.Item1.sslg_length)))
               //.ForMember(x => x.ExternalSpotRef, expression => expression.MapFrom(spotImport => ""))   // Blank, was spotImport.Item1.spot_no.ToString()
               //.ForMember(x => x.SpotRating, expression => expression.MapFrom(spotImport => spotImport.Item1.rating))
               .ForMember(x => x.SpotRating, expression => expression.MapFrom(spotImport => 0))
               .ForMember(x => x.Action, expression => expression.MapFrom(spotImport => GetAction(spotImport.Item1)))
               .ForMember(x => x.Processor, expression => expression.MapFrom(spotImport => GetProcessor(spotImport.Item1)))
               .ForMember(x => x.ProcessorDateTime, opts => opts.Ignore())     // Manually
                                                                               //.ForMember(x => x.GroupCode, expression => expression.MapFrom(spotImport => spotImport.Item1.spot_sare_no))
                                                                               // ForMember(x => x.ClientPicked, expression => expression.MapFrom(spotImport => true))           // Correct? (SPOTS.OUT has client_picked)
                                                                               //.ForMember(x => x.MultipartSpot, expression => expression.MapFrom(spotImport => spotImport.Item1.mpart_spot_ind))
                                                                               //.ForMember(x => x.MultipartSpotPosition, expression => expression.MapFrom(spotImport => spotImport.Item1.bkpo_posn_reqm))
                                                                               //.ForMember(x => x.MultipartSpotRef, expression => expression.MapFrom(spotImport => ""))
               .ForMember(x => x.RequestedPositionInBreak, expression => expression.MapFrom(spotImport => GetRequestedPositionInBreak(spotImport.Item1.bkpo_posn_reqm.GetValueOrDefault(0))))
               .ForMember(x => x.ActualPositionInBreak, expression => expression.MapFrom(spotImport => GetRequestedPositionInBreak(spotImport.Item1.bkpo_posn_reqm.GetValueOrDefault(0))))
               //.ForMember(x => x.ExternalBreakNo, expression => expression.MapFrom(spotImport => ""))      // Correct? (SPOTS.OUT has break_no)
               .ForMember(x => x.Filler, expression => expression.MapFrom(spotImport => false))
               .ForMember(x => x.Sponsored, expression => expression.MapFrom(spotImport => false))     // Spots.Sponsored
               .ForMember(x => x.Preemptable, expression => expression.MapFrom(spotImport => false))
               .ForMember(x => x.Preemptlevel, expression => expression.MapFrom(spotImport => 0));
        }

        private DateTime GetStartDateTime(SpotReqmImport spotImport)
        {
            string nom_time = spotImport.brek_nom_time.ToString("000000");
            int seconds = (Convert.ToInt32(nom_time.Substring(0, 2)) * (60 * 60)) +     // Hours
                          (Convert.ToInt32(nom_time.Substring(2, 2)) * 60) +            // Mins
                          (Convert.ToInt32(nom_time.Substring(4, 2)));                  // Secs

            return DateHelper.GetDateTime(string.Format("{0} {1}", spotImport.brek_sched_date.ToString("00000000"), "000000"), "yyyyMMdd HHmmss", DateTimeKind.Utc).AddSeconds(seconds);
        }

        private DateTime GetStartDateTime(SpotImport spotImport)
        {
            string nom_time = spotImport.brek_nom_time.ToString("000000");
            int seconds = (Convert.ToInt32(nom_time.Substring(0, 2)) * (60 * 60)) +     // Hours
                          (Convert.ToInt32(nom_time.Substring(2, 2)) * 60) +            // Mins
                          (Convert.ToInt32(nom_time.Substring(4, 2)));                  // Secs
            return DateHelper.GetDateTime(string.Format("{0} {1}", spotImport.brek_sched_date.ToString("00000000"), "000000"), "yyyyMMdd HHmmss", DateTimeKind.Utc).AddSeconds(seconds);
        }

        private DateTime GetEndDateTime(SpotReqmImport spotImport, Duration duration)
        {
            return GetStartDateTime(spotImport).AddTicks(duration.BclCompatibleTicks);
        }

        private string GetAction(SpotReqmImport spotImport)
        {
            if (!String.IsNullOrEmpty(spotImport.status) && spotImport.status.ToUpper() == "C")
            {
                return "C";
            }
            return "B";
        }

        private string GetProcessor(SpotReqmImport spotImport)
        {
            if (!String.IsNullOrEmpty(spotImport.status) && spotImport.status.ToUpper() == "C")
            {
                switch (spotImport.abdn_no)
                {
                    case -2: return RsProcessor;
                    case -1: return IsrProcessor;
                }
            }
            return AutoProcessor;
        }

        private string GetProcessor(SpotImport spotImport)
        {
            if (!String.IsNullOrEmpty(spotImport.status) && spotImport.status.ToUpper() == "C")
            {
                switch (spotImport.pass_no)
                {
                    case -2: return RsProcessor;
                    case -1: return IsrProcessor;
                }
            }
            return AutoProcessor;
        }

        private string GetAction(SpotImport spotImport)
        {
            if (!String.IsNullOrEmpty(spotImport.status) && spotImport.status.ToUpper() == "C")
            {
                return "C";
            }
            return "B";
        }

        private string GetBreakType(string breakTypeCode, IEnumerable<string> breakTypes)
        {
            if(String.IsNullOrWhiteSpace(breakTypeCode) || breakTypeCode.Length < 2)
            {
                return null;
            }
            return breakTypes?.FirstOrDefault(b => b.StartsWith(breakTypeCode.Substring(0, 2)));
        }

        private string GetRequestedPositionInBreak(int position)
        {
            switch (position)
            {
                case 1: 
                    return PositionInBreakRequests.First;
                case 2: 
                    return PositionInBreakRequests.SecondFromStart;
                case 3: 
                    return PositionInBreakRequests.ThirdFromStart;
                case 97: 
                    return PositionInBreakRequests.ThirdFromLast;
                case 98: 
                    return PositionInBreakRequests.SecondFromLast;
                case 99: 
                    return PositionInBreakRequests.Last;
                default:
                    return null; 
            }
        }

        private string GetExternalProgrammeReference(long programmeNo, List<ProgrammeDictionary> programmeDictionaries)
        {
            return programmeDictionaries?.FirstOrDefault(p => p.Id.Equals((int)programmeNo))?.ExternalReference ??
                   string.Empty;
        }

        private string GetProgrammeName(long programmeNumber, List<ProgrammeDictionary> programmeDictionaries)
        {
            return programmeDictionaries?.FirstOrDefault(p => p.Id.Equals((int)programmeNumber))?.ProgrammeName ??
                   string.Empty;
        }

        private int GetIntOrDefault(string input, int defaultValue) => int.TryParse(input, out int value) ? value : defaultValue;
    }
}
