using System;
using Hal;

namespace JsonHalTests.Model
{
    [HalContract]
    public class Session : IHalEntity
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
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        [HalMember(Name = "id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        [HalEmbedded(Rel = "user")]
        public User User { get; set; }

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
    }
}