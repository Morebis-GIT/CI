namespace ImagineCommunications.GamePlan.Domain.Shared.Products.Objects
{
    public class ProductAdvertiserModel
    {
        public string AdvertiserIdentifier { get; set; }
        public string AdvertiserName { get; set; }

        #region Equals
        public override bool Equals(object obj)
        {
            if (obj is ProductAdvertiserModel model)
            {
                return Equals(model);
            }

            return false;
        }

        protected bool Equals(ProductAdvertiserModel other)
        {
            return other != null &&
                   string.Equals(AdvertiserIdentifier, other.AdvertiserIdentifier) &&
                   string.Equals(AdvertiserName, other.AdvertiserName);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((AdvertiserIdentifier != null ? AdvertiserIdentifier.GetHashCode() : 0) * 397) ^
                       (AdvertiserName != null ? AdvertiserName.GetHashCode() : 0);
            }
        }
        #endregion
    }
}
