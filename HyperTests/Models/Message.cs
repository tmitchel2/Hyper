using Hyper;

namespace HyperTests.Models
{
    [HyperContract(Name = "message", MediaType = "application/vnd.hypertests.message", Version = "1.0.0.0")]
    public class Message : IHyperEntity<Message>
    {
        public readonly static Message Empty = new Message();

        /// <summary>
        /// Initializes a new instance of the <see cref="User" /> class.
        /// </summary>
        public Message()
        {
            Self = HyperLink<Message>.Empty;
        }

        /// <summary>
        /// Gets or sets the self.
        /// </summary>
        /// <value>
        /// The self.
        /// </value>
        [HyperLink(Rel = "self")]
        public HyperLink<Message> Self { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        [HyperMember(Name = "text")]
        public string Text { get; set; }
    }
}