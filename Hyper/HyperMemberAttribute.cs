using System;

namespace Hyper
{
    /// <summary>
    /// HyperMemberAttribute class.
    /// </summary>
    public class HyperMemberAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [HyperMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the hyper.
        /// </summary>
        /// <value>
        /// The type of the hyper.
        /// </value>
        [HyperMember(Name = "type")]
        public HyperType Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is optional.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is optional; otherwise, <c>false</c>.
        /// </value>
        [HyperMember(Name = "optional")]
        public bool IsOptional { get; set; }
    }
}