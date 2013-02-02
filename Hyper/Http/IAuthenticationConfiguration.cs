using System.IdentityModel.Selectors;

namespace Hyper.Http
{
    /// <summary>
    /// IAuthenticationConfiguration interface.
    /// </summary>
    public interface IAuthenticationConfiguration
    {
        /// <summary>
        /// Gets the name of the authentication cookie.
        /// </summary>
        /// <value>
        /// The name of the authentication cookie.
        /// </value>
        string AuthenticationCookieName { get; }

        /// <summary>
        /// Gets the web API user name password validator.
        /// </summary>
        /// <value>
        /// The web API user name password validator.
        /// </value>
        UserNamePasswordValidator WebApiUserNamePasswordValidator { get; }
    }
}