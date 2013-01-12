namespace Hyper.Http
{
    /// <summary>
    /// IHasBasicAuthicationDetails interface.
    /// </summary>
    public interface IHasBasicAuthicationDetails
    {
        /// <summary>
        /// Gets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        string Username { get; }

        /// <summary>
        /// Gets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        string Password { get; }
    }
}
