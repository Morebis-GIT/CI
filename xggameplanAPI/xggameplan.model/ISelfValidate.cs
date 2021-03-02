namespace xggameplan.model
{
    public interface ISelfValidate
    {
        /// <summary>Validates this instance follows business rules for the
        /// object to be created. If the validation fails an exception will be
        /// thrown.</summary>
        void Validate();
    }
}
