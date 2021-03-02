using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ClashExceptions;
using ImagineCommunications.Gameplan.Integration.Handlers.Common;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Products;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.ClashExceptions
{
    public class BulkClashExceptionCreatedEventHandler : BusClient.Abstraction.Classes.EventHandler<IBulkClashExceptionCreated>
    {
        private readonly IMapper _mapper;
        private readonly IClashExceptionRepository _clashExceptionRepository;
        private readonly IClashExceptionValidations _clashExceptionValidations;
        private readonly IClashRepository _clashRepository;
        private readonly IProductRepository _productRepository;

        public BulkClashExceptionCreatedEventHandler(IMapper mapper,
            IClashExceptionRepository clashExceptionRepository,
            IClashExceptionValidations clashExceptionValidations,
            IClashRepository clashRepository,
            IProductRepository productRepository)
        {
            _clashRepository = clashRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _clashExceptionValidations = clashExceptionValidations;
            _clashExceptionRepository = clashExceptionRepository;
        }

        public override void Handle(IBulkClashExceptionCreated command)
        {
            var clashExceptions = _mapper.Map<List<ClashException>>(command.Data);
            foreach (var item in clashExceptions)
            {
                item.StartDate = item.StartDate.Date;
                item.EndDate = item.EndDate?.Date;
                _clashExceptionRepository.Delete(item.FromType, item.ToType, item.FromValue, item.ToValue);
            }

            ValidateFromToValue(clashExceptions);
            ValidateForSave(clashExceptions);

            _clashExceptionRepository.Add(clashExceptions);
            _clashExceptionRepository.SaveChanges();
        }

        private void ValidateFromToValue(List<ClashException> command)
        {
            var clashFromCode = command
                .Where(x => x.FromType == ClashExceptionType.Clash)
                .Select(x => x.FromValue.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase);

            var clashToCode = command
                .Where(x => x.ToType == ClashExceptionType.Clash)
                .Select(x => x.ToValue.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase);

            var clashCode = clashFromCode.Union(clashToCode).ToList();
            if (clashCode.Any())
            {
                ValidateClashCodes(clashCode);
            }

            var productFromCode = command
                .Where(x => x.FromType == ClashExceptionType.Product)
                .Select(x => x.FromValue.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase);

            var productToCode = command
                .Where(x => x.ToType == ClashExceptionType.Product)
                .Select(x => x.ToValue.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase);

            var productCode = productFromCode.Union(productToCode).ToList();
            if (productCode.Any())
            {
                ValidateProductExternalIds(productCode);
            }

            var advertiserFromCode = command
                .Where(x => x.FromType == ClashExceptionType.Advertiser)
                .Select(x => x.FromValue.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase);

            var advertiserToCode = command
                .Where(x => x.ToType == ClashExceptionType.Advertiser)
                .Select(x => x.ToValue.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase);

            var advertiserCode = advertiserFromCode.Union(advertiserToCode).ToList();
            if (advertiserCode.Any())
            {
                ValidateProductAdvertiserIds(advertiserCode);
            }
        }

        private void ValidateClashCodes(List<string> clashCodes)
        {
            var clashes = _clashRepository
                .FindByExternal(clashCodes)
                .Select(x => x.Externalref)
                .Distinct(StringComparer.CurrentCultureIgnoreCase);

            var invalidClashCodes = clashCodes.Except(clashes, StringComparer.CurrentCultureIgnoreCase).ToList();
            if (invalidClashCodes.Any())
            {
                throw new DataSyncException(DataSyncErrorCode.ClashCodeNotFound, $"Invalid clash code: {string.Join(", ", invalidClashCodes)}");
            }
        }

        private void ValidateProductExternalIds(List<string> productExternalIds)
        {
            var externalIds = _productRepository
                .FindByExternal(productExternalIds)
                .Select(x => x.Externalidentifier)
                .Distinct(StringComparer.CurrentCultureIgnoreCase);

            var invalidProductExternalIds = productExternalIds.Except(externalIds, StringComparer.CurrentCultureIgnoreCase).ToList();
            if (invalidProductExternalIds.Any())
            {
                throw new DataSyncException(DataSyncErrorCode.ProductNotFound, $"Invalid Product: {string.Join(", ", invalidProductExternalIds)}");
            }
        }

        private void ValidateProductAdvertiserIds(List<string> productAdvertiserIds)
        {
            var externalIds = _productRepository
                .FindByAdvertiserId(productAdvertiserIds)
                .Select(p => p.AdvertiserIdentifier)
                .Distinct();

            var invalidProductAdvertiserIds = productAdvertiserIds.Except(externalIds, StringComparer.CurrentCultureIgnoreCase).ToList();
            if (invalidProductAdvertiserIds.Any())
            {
                throw new DataSyncException(DataSyncErrorCode.ProductAdvertiserNotFound,
                    $"Invalid Product Advertiser id: {string.Join(", ", invalidProductAdvertiserIds.ToList())}");
            }
        }

        private void ValidateForSave(List<ClashException> incomingClashExceptions)
        {
            foreach (var clashException in incomingClashExceptions)
            {
                var structureRulesViolationValidationResult = _clashExceptionValidations.ValidateClashExceptionForSameStructureRulesViolation(clashException);
                if (!structureRulesViolationValidationResult.Successful)
                {
                    throw new DataSyncException(DataSyncErrorCode.ClashException_StructureRulesViolation, structureRulesViolationValidationResult.Message);
                }
            }

            const int offsetHours = 6;
            var existingClashExceptions = _clashExceptionRepository.GetActive().ToList();

            var timeRangesValidationResult = _clashExceptionValidations.ValidateTimeRanges(incomingClashExceptions, offsetHours, existingClashExceptions);
            if (!timeRangesValidationResult.Successful)
            {
                throw new DataSyncException(DataSyncErrorCode.ClashException_TimeRangesOverlap, timeRangesValidationResult.Message);
            }
        }
    }
}
