using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects.AgBusinessType;
using ImagineCommunications.GamePlan.Domain.BusinessTypes.Objects;
using xggameplan.Extensions;
using xggameplan.Model.AutoGen.AgBusinessTypes;

namespace xggameplan.core.Services.OptimiserInputFilesSerialisers
{
    public class BusinessTypeSerializer
    {
        private string FolderName { get; }

        public BusinessTypeSerializer(string folderName)
        {
            if (string.IsNullOrWhiteSpace(folderName))
            {
                throw new ArgumentNullException(nameof(folderName));
            }

            FolderName = folderName;
        }

        public static string Filename => "v_bstp_list.xml";

        public void Serialise(List<BusinessType> businessTypes)
        {
            ToAgBusinessType(businessTypes)
                .Serialize(Path.Combine(FolderName, Filename));
        }

        private AgBusinessTypesSerialization ToAgBusinessType(List<BusinessType> businessTypes)
        {
            var agBusinessTypes = businessTypes
                .Select(x => new AgBusinessType { Code = x.Code })
                .ToList();

            var serialization = new AgBusinessTypesSerialization();

            serialization.MapFrom(agBusinessTypes);

            return serialization;
        }
    }
}
