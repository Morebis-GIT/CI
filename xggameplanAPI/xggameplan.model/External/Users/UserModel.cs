namespace xggameplan.Model
{
    /// <summary>
    /// Representation of the user.
    /// </summary>
    public class UserModel : UserReducedModel
    {
        /// <summary>
        /// Surname.
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Email which also works as a login.
        /// </summary>
        public string Email { get; set; }

    }
}
