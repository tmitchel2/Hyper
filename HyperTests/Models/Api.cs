using System;
using Hyper;

namespace HyperTests.Models
{
    /// <summary>
    /// Api class.
    /// </summary>
    [HyperContract(Name = "api", MediaType = "application/vnd.hypertests.api", Version = "1.0.0.0")]
    public class Api : IHyperEntity
    {
        /// <summary>
        /// Gets or sets the self.
        /// </summary>
        /// <value>
        /// The self.
        /// </value>
        [HyperLink(Rel = "self")]
        public HyperLink Self { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [HyperMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        [HyperMember(Name = "version")]
        public Version Version { get; set; }

        /// <summary>
        /// Gets or sets the types.
        /// </summary>
        /// <value>
        /// The types.
        /// </value>
        [HyperLink(Rel = "types")]
        public HyperLinkList<HyperType> Types { get; set; }
        
        /// <summary>
        /// Gets or sets the sessions.
        /// </summary>
        /// <value>
        /// The sessions.
        /// </value>
        [HyperLink(Rel = "sessions")]
        public HyperLinkList<Session> Sessions { get; set; }

        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        /// <value>
        /// The users.
        /// </value>
        [HyperLink(Rel = "users")]
        public HyperLinkList<User> Users { get; set; }

        /// <summary>
        /// Gets or sets the messages.
        /// </summary>
        /// <value>
        /// The messages.
        /// </value>
        [HyperLink(Rel = "messages")]
        public HyperLinkList<Message> Messages { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Self != null ? Self.Href : "Api";
        }
    }
}