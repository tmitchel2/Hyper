using System.Collections.Generic;
using System.Linq;

namespace Hyper
{
    /// <summary>
    /// HyperType class.
    /// </summary>
    [HyperContract(Name = "hypertype", MediaType = "application/vnd.hyper.hypertype", Version = "1.0.0.0")]
    public class HyperType
    {
        public static readonly HyperType String = new HyperType(new HyperLink("/hyper/types/string"), "string");
        
        /// <summary>
        /// Initializes a new instance of the <see cref="HyperType" /> class.
        /// </summary>
        public HyperType()
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="HyperType" /> class.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <param name="name">The name.</param>
        /// <param name="members">The members.</param>
        /// <param name="links">The links.</param>
        public HyperType(HyperLink self, string name, IEnumerable<HyperMember> members = null, IEnumerable<HyperLink> links = null)
        {
            Self = self;
            Name = name;
            Members = members != null ? members.ToList() : new List<HyperMember>();
            Links = links != null ? links.ToList() : new List<HyperLink>();
        }

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
        /// Gets or sets the type of the media.
        /// </summary>
        /// <value>
        /// The type of the media.
        /// </value>
        [HyperMember(Name = "media-type")]
        public string MediaType { get; set; }

        /// <summary>
        /// Gets or sets the members.
        /// </summary>
        /// <value>
        /// The members.
        /// </value>
        [HyperEmbedded(Rel = "members")]
        public IList<HyperMember> Members { get; set; }

        /// <summary>
        /// Gets or sets the links.
        /// </summary>
        /// <value>
        /// The links.
        /// </value>
        [HyperEmbedded(Rel = "links")]
        public IList<HyperLink> Links { get; set; }
    }
}