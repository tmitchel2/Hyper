using Hyper;
using Hyper.Http;

namespace HyperTests.Models
{
    [HyperContract(Name = "session", MediaType = "application/vnd.hypertests.session", Version="1.0.0.0")]
    public class Session : IHyperEntity<Session>, IHasBasicAuthicationDetails
    {
        public readonly static Session Empty = new Session();

        public Session()
        {
            Self = HyperLink<Session>.Empty;
            Id = 0;
            User = HyperLink<User>.Empty;
        }

        /// <summary>
        /// Gets or sets the self.
        /// </summary>
        /// <value>
        /// The self.
        /// </value>
        [HyperLink(Rel = "self")]
        public HyperLink<Session> Self { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        [HyperMember(Name = "id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        [HyperMember(Name = "username")]
        public string Username { get; set; }

        /// <summary>
        /// Gets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        [HyperMember(Name = "password", IsOptional = true)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        [HyperLink(Rel = "user")]
        public HyperLink<User> User { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Self != null ? Self.Href : "Session";
        }

        /// <summary>
        /// Gets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        string IHasBasicAuthicationDetails.Username 
        {
            get
            {
                return Username;
            }
        }

        /// <summary>
        /// Gets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        string IHasBasicAuthicationDetails.Password
        {
            get
            {
                return Password;
            }
        }
    }
}