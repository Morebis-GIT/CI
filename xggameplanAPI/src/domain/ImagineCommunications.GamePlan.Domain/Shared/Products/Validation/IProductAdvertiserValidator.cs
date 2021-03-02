using System.Collections.Generic;

namespace xggameplan.core.Validators.ProductAdvertiser
{
    /// <summary>Validates product advertisers.</summary>
    public interface IProductAdvertiserValidator
    {
        /// <summary>Validates the specified advertiser period infos.</summary>
        /// <param name="advertiserPeriodInfos">The advertiser period infos.</param>
        void Validate(IReadOnlyCollection<IAdvertiserPeriodInfo> advertiserPeriodInfos);
    }
}
