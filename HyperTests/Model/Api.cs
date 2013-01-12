using System;
using Hal;

namespace JsonHalTests.Model
{
    /// <summary>
    /// Api class.
    /// </summary>
    [HalContract]
    public class Api : IHalEntity
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
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [HalMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        [HalMember(Name = "version")]
        public Version Version { get; set; }

        /// <summary>
        /// Gets or sets the types.
        /// </summary>
        /// <value>
        /// The types.
        /// </value>
        [HalLink(Rel = "types")]
        public HalLinkList<HalType> Types { get; set; }
        
        /// <summary>
        /// Gets or sets the sessions.
        /// </summary>
        /// <value>
        /// The sessions.
        /// </value>
        [HalLink(Rel = "sessions")]
        public HalLinkList<Session> Sessions { get; set; }

        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        /// <value>
        /// The users.
        /// </value>
        [HalLink(Rel = "users")]
        public HalLinkList<User> Users { get; set; }

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