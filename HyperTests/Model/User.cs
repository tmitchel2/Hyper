using Hal;

namespace JsonHalTests.Model
{
    /// <summary>
    /// User class.
    /// </summary>
    [HalContract]
    public class User : IHalEntity
    {
        /// <summary>
        /// Gets or sets the self.
        /// </summary>
        /// <value>
        /// The self.
        /// </value>
        [HalLink(Rel = "self")]
        public HalLink Self { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        [HalMember(Name = "email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        [HalMember(Name = "password")]
        public string Password { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Self != null ? Self.Href : "User";
        }
    }
}