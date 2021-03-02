using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using NodaTime;
using xggameplan.AuditEvents;
using xggameplan.core.Services.OptimiserInputFilesSerialisers.Interfaces;
using xggameplan.Extensions;
using xggameplan.Model.AutoGen;

namespace xggameplan.core.Services.OptimiserInputFilesSerialisers
{
    /// <summary>
    /// Serializers clash exceptions into xml file.
    /// </summary>
    /// <seealso cref="xggameplan.core.Services.OptimiserInputFilesSerialisers.SerializerBase" />
    /// <seealso cref="xggameplan.core.Services.OptimiserInputFilesSerialisers.Interfaces.IClashExceptionSerializer" />
    public class ClashExceptionSerializer : SerializerBase, IClashExceptionSerializer
    {
        private const string AgDateFormat = "yyyyMMdd";
        private const string AgTimeFormat = "hhmmss";
        private const string DefaultAgMinTime = "0";
        private const string DefaultAgMaxTime = "995959";

        private readonly IClashExceptionRepository _clashExceptionRepository;
        private readonly IProductRepository _productRepository;
        private readonly IClock _clock;

        /// <summary>Initializes a new instance of the <see cref="ClashExceptionSerializer" /> class.</summary>
        /// <param name="clashExceptionRepository">The clash exception repository.</param>
        /// <param name="productRepository">The product repository.</param>
        /// <param name="auditEventRepository">The audit event repository.</param>
        /// <param name="clock">The clock.</param>
        public ClashExceptionSerializer(
            IClashExceptionRepository clashExceptionRepository,
            IProductRepository productRepository,
            IAuditEventRepository auditEventRepository,
            IClock clock) : base(auditEventRepository)
        {
            _clashExceptionRepository = clashExceptionRepository;
            _productRepository = productRepository;
            _clock = clock;
        }

        /// <summary>Gets the filename.</summary>
        /// <value>The filename.</value>
        public string Filename => "v_clsh_exc_list.xml";

        /// <summary>Serializes clash exceptions.</summary>
        /// <param name="folderName">Name of the folder.</param>
        /// <param name="run">The run.</param>
        /// <param name="clashes">The clashes.</param>
        /// <param name="filteredProducts">List of products filtered by included campaign list.</param>
        public void Serialize(string folderName, Run run, IReadOnlyCollection<Clash> clashes, IReadOnlyCollection<Product> filteredProducts)
        {
            var allClashExceptions = _clashExceptionRepository.GetAll().ToList();

            if (allClashExceptions.Count == 0)
            {
                return;
            }

            var filteredClashExceptions = new List<ClashException>();

            foreach (var product in filteredProducts)
            {
                var validClashExceptionsByProduct = allClashExceptions.Where(o =>
                    (o.FromType == ClashExceptionType.Product && o.FromValue == product.Externalidentifier) ||
                    (o.ToType == ClashExceptionType.Product && o.ToValue == product.Externalidentifier));
                var validClashExceptionsByClash = allClashExceptions.Where(o =>
                    (o.FromType == ClashExceptionType.Clash && product.ClashCode.Equals(o.FromValue, StringComparison.OrdinalIgnoreCase)) ||
                    (o.ToType == ClashExceptionType.Clash && product.ClashCode.Equals(o.ToValue, StringComparison.OrdinalIgnoreCase)));
                var validClashExceptionsByAdvertiser = allClashExceptions.Where(o =>
                    (o.FromType == ClashExceptionType.Advertiser && product.AdvertiserIdentifier.Equals(o.FromValue, StringComparison.OrdinalIgnoreCase)) ||
                    (o.ToType == ClashExceptionType.Advertiser && product.AdvertiserIdentifier.Equals(o.ToValue, StringComparison.OrdinalIgnoreCase)));
                filteredClashExceptions = filteredClashExceptions
                    .Union(validClashExceptionsByProduct)
                    .Union(validClashExceptionsByClash)
                    .Union(validClashExceptionsByAdvertiser)
                    .ToList();
            }

            RaiseInfo(
                $"Started populating {Filename}. Total clash exceptions - {filteredClashExceptions.Count}, Current Time - {_clock.GetCurrentInstant().ToDateTimeUtc()}");

            var products = GetProducts(filteredClashExceptions);
            var agClashExceptions = filteredClashExceptions.SelectMany(clashException =>
            {
                GetClashProductCode(clashException, products, clashes, out List<string> fromCodes,
                    out List<string> toCodes);

                return (from timeAndDow in clashException.TimeAndDows // for each Time and DOW row
                        from dayRange in timeAndDow.DaysOfWeek?.GetDayRangeFromDayCode() ??
                                         throw new Exception(
                                             "No Time and Dow values") // for each day ranges eg [1,3],[5,6],[7,7]
                        from fromCode in fromCodes
                        from toCode in toCodes
                        select new AgClashException
                        {
                            FromClashCode = clashException.FromType == ClashExceptionType.Clash ? fromCode : "",
                            ToClashCode = clashException.ToType == ClashExceptionType.Clash ? toCode : "",
                            FromProductCode = clashException.FromType != ClashExceptionType.Clash
                                ? Convert.ToInt32(fromCode)
                                : 0,
                            ToProductCode = clashException.ToType != ClashExceptionType.Clash
                                ? Convert.ToInt32(toCode)
                                : 0,
                            DefinedClash = 1,
                            EndDate =
                                (clashException.EndDate == null
                                    ? run.EndDate.Date.AddYears(10).ToString(AgDateFormat)
                                    : clashException.EndDate.Value.ToString(AgDateFormat)),
                            StartDate = clashException.StartDate.ToString(AgDateFormat),
                            IncludeExcludeFlag = clashException.IncludeOrExclude.ToString().ToUpper(),
                            StartTime = timeAndDow.StartTime?.ToString(AgTimeFormat) ?? DefaultAgMinTime,
                            EndTime = timeAndDow.EndTime?.ToString(AgTimeFormat) ?? DefaultAgMaxTime,
                            StartDayNo = dayRange.Item1,
                            EndDayNo = dayRange.Item2
                        }).ToList();
            }).ToList();

            new AgClashExceptionsSerialisation().MapFrom(agClashExceptions)
                .Serialize(Path.Combine(folderName, Filename));

            RaiseInfo(
                $"Finished populating {Filename}. Total clash exceptions - {filteredClashExceptions.Count}, Current Time - {_clock.GetCurrentInstant().ToDateTimeUtc()}");
        }

        /// <summary>Gets the products.</summary>
        /// <param name="clashExceptions">The clash exceptions.</param>
        /// <returns></returns>
        private List<Product> GetProducts(IReadOnlyCollection<ClashException> clashExceptions)
        {
            var advertiserFromCode = clashExceptions.Where(_ => _.FromType == ClashExceptionType.Advertiser)
                .Select(_ => _.FromValue);
            var advertiserToCode = clashExceptions.Where(_ => _.ToType == ClashExceptionType.Advertiser)
                .Select(_ => _.ToValue);

            var advertiserCode = advertiserFromCode.Union(advertiserToCode).Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            return advertiserCode.Count > 0
                ? _productRepository.FindByAdvertiserId(advertiserCode).ToList()
                : new List<Product>();
        }

        /// <summary>Gets the clash product code.</summary>
        /// <param name="clashException">The clash exception.</param>
        /// <param name="products">The products.</param>
        /// <param name="clashes">The clashes.</param>
        /// <param name="fromCodes">From codes.</param>
        /// <param name="toCodes">To codes.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void GetClashProductCode(ClashException clashException, IReadOnlyCollection<Product> products,
            IReadOnlyCollection<Clash> clashes,
            out List<string> fromCodes, out List<string> toCodes)
        {
            switch (clashException.FromType)
            {
                case ClashExceptionType.Clash:
                    fromCodes = GetRelatedClashCodes(clashes, clashException.FromValue) ??
                                new List<string> { clashException.FromValue };
                    break;

                case ClashExceptionType.Product:
                    fromCodes = new List<string> { clashException.FromValue };
                    break;

                case ClashExceptionType.Advertiser:
                    fromCodes = products?
                                    .Where(_ => _.AdvertiserIdentifier.Equals(clashException.FromValue,
                                        StringComparison.OrdinalIgnoreCase))
                                    .Select(_ => _.Externalidentifier).ToList() ??
                                new List<string> { "0" }; // if No products return list with default value
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (clashException.ToType)
            {
                case ClashExceptionType.Clash:
                    toCodes = GetRelatedClashCodes(clashes, clashException.ToValue) ??
                              new List<string> { clashException.ToValue };
                    break;

                case ClashExceptionType.Product:
                    toCodes = new List<string> { clashException.ToValue };
                    break;

                case ClashExceptionType.Advertiser:
                    toCodes = products?
                                  .Where(_ => _.AdvertiserIdentifier.Equals(clashException.ToValue,
                                      StringComparison.OrdinalIgnoreCase))
                                  .Select(_ => _.Externalidentifier).ToList()
                              ?? new List<string> { "0" }; // if No products return list with default value
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>Gets the related clash codes.</summary>
        /// <param name="clashes">The clashes.</param>
        /// <param name="parentClashCode">The parent clash code.</param>
        /// <returns></returns>
        private List<string> GetRelatedClashCodes(IReadOnlyCollection<Clash> clashes, string parentClashCode)
        {
            return clashes?
                .Where(c => c.Externalref == parentClashCode || c.ParentExternalidentifier == parentClashCode)
                .Select(c => c.Externalref)
                .ToList();
        }
    }
}
