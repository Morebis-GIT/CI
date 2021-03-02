using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;

namespace xggameplan.Extensions
{
    /// <summary>
    /// Extends <see cref="Product"/> functionality.
    /// </summary>
    public static class ProductExtensions
    {
        /// <summary>
        /// Updates the <see cref="Product"/> properties by the values of the specified <see cref="Product"/> parameter.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        public static Product Update(this Product self, Product product)
        {
            self.ParentExternalidentifier = product.ParentExternalidentifier;
            self.Name = product.Name;
            self.EffectiveStartDate = product.EffectiveStartDate;
            self.EffectiveEndDate = product.EffectiveEndDate;
            self.ClashCode = product.ClashCode;
            self.AdvertiserIdentifier = product.AdvertiserIdentifier;
            self.AdvertiserName = product.AdvertiserName;
            self.AdvertiserLinkStartDate = product.AdvertiserLinkStartDate;
            self.AdvertiserLinkEndDate = product.AdvertiserLinkEndDate;
            self.AgencyIdentifier = product.AgencyIdentifier;
            self.AgencyName = product.AgencyName;
            self.AgencyStartDate = product.AgencyStartDate;
            self.AgencyLinkEndDate = product.AgencyLinkEndDate;

            return self;
        }
    }
}
