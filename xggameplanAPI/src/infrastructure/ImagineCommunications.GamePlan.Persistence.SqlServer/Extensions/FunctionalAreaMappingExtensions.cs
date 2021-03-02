using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Dto.Internal;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions
{
    internal static class FunctionalAreaMappingExtensions
    {
        private const string FaultTypeSelectionValue = "FaultTypeSelectionValue";

        public static IEnumerable<FunctionalArea> ApplyFaultTypes(this IQueryable<FunctionalAreaDto> query,
            IDictionary<int, Entities.Tenant.FunctionalAreas.FaultType> faultTypes, IMapper mapper)
        {
            return query?.AsEnumerable().Select(x => x.ApplyFaultTypes(faultTypes, mapper));
        }

        public static FunctionalArea ApplyFaultTypes(this FunctionalAreaDto entity,
            IDictionary<int, Entities.Tenant.FunctionalAreas.FaultType> faultTypes, IMapper mapper)
        {
            if (entity is null)
            {
                return null;
            }

            entity.FaultTypes = entity.FaultTypeSelections.Select(s =>
            {
                if (faultTypes.TryGetValue(s.Key, out var faultType))
                {
                    return (FaultType)mapper.Map<FaultTypeDto>(faultType, opt => opt.WithSelectionValue(s.Value));
                }
                return null;

            }).Where(s => s != null).ToList();

            return entity;
        }

        public static IMappingOperationOptions WithSelectionValue(
            this IMappingOperationOptions opts, bool value)
        {
            if (opts == null)
            {
                return null;
            }

            if (!opts.Items.ContainsKey(FaultTypeSelectionValue))
            {
                opts.Items.Add(FaultTypeSelectionValue, value);
            }

            return opts;
        }

        public static bool TryGetFaultTypeSelectionValue(this IMappingOperationOptions opts, out bool value)
        {
            value = false;

            if (opts is null)
            {
                return false;
            }

            if (opts.Items.TryGetValue(FaultTypeSelectionValue, out var faultTypeSelectionValue))
            {
                value = (bool)faultTypeSelectionValue;
                return true;
            }

            return false;
        }
    }
}
