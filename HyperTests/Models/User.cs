using Hyper;

namespace HyperTests.Models
{
    /// <summary>
    /// User class.
    /// </summary>
    [HyperContract(Name = "user", MediaType = "application/vnd.hypertests.user", Version = "1.0.0.0")]
    public class User : IHyperEntity<User>
    {
        public readonly static User Empty = new User();

        /// <summary>
        /// Initializes a new instance of the <see cref="User" /> class.
        /// </summary>
        public User()
        {
            Self = HyperLink<User>.Empty;
        }

        /// <summary>
        /// Gets or sets the self.
        /// </summary>
        /// <value>
        /// The self.
        /// </value>
        [HyperLink(Rel = "self")]
        public HyperLink<User> Self { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        [HyperMember(Name = "email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        [HyperMember(Name = "password")]
        public string Password { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var href = Self != null ? Self.Href : null;
            return href ?? Email ?? "User";
        }
    }
}