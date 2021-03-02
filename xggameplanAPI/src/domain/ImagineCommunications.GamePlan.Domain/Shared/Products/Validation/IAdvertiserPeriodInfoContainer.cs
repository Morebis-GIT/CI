using System.Collections.Generic;

namespace xggameplan.core.Validators.ProductAdvertiser
{
    /// <summary>
    /// Exposes the collection of <see cref="IAdvertiserPeriodInfo"/>
    /// </summary>
    public interface IAdvertiserPeriodInfoContainer
    {
        /// <summary>
        ///   <para>
        ///  Gets the advertiser period information.
        /// </para>
        /// </summary>
        /// <returns></returns>
        IEnumerable<IAdvertiserPeriodInfo> GetAdvertiserPeriodInfo();
    }
}
