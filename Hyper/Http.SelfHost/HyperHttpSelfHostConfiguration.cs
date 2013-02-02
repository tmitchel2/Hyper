using System.IdentityModel.Selectors;
using System.Web.Http.SelfHost;
using System.Web.Http.SelfHost.Channels;

namespace Hyper.Http.SelfHost
{
    /// <summary>
    /// HyperHttpSelfHostConfiguration class.
    /// </summary>
    public class HyperHttpSelfHostConfiguration : HttpSelfHostConfiguration, IAuthenticationConfiguration
    {
        private readonly bool _requiresSSL;

        /// <summary>
        /// Initializes a new instance of the <see cref="HyperHttpSelfHostConfiguration" /> class.
        /// </summary>
        /// <param name="baseAddress">The base address.</param>
        /// <param name="requiresSSL">if set to <c>true</c> [requires SSL].</param>
        public HyperHttpSelfHostConfiguration(string baseAddress, bool requiresSSL)
            : base(baseAddress)
        {
            _requiresSSL = requiresSSL;
        }

        /// <summary>
        /// Gets or sets the web API user name password validator.
        /// </summary>
        /// <value>
        /// The web API user name password validator.
        /// </value>
        public UserNamePasswordValidator WebApiUserNamePasswordValidator { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the authentication cookie.
        /// </summary>
        /// <value>
        /// The name of the authentication cookie.
        /// </value>
        public string AuthenticationCookieName { get; set; }

        /// <summary>
        /// Gets the name of the session cookie.
        /// </summary>
        /// <value>
        /// The name of the session cookie.
        /// </value>
        public string SessionCookieName
        {
            get
            {
                return AuthenticationCookieName + "-sessionid";
            }
        }

        /// <summary>
        /// Called to apply the configuration on the endpoint level.
        /// </summary>
        /// <param name="httpBinding">The HTTP endpoint.</param>
        /// <returns>
        /// The <see cref="T:System.ServiceModel.Channels.BindingParameterCollection" /> to use when building the <see cref="T:System.ServiceModel.Channels.IChannelListener" /> or null if no binding parameters are present.
        /// </returns>
        protected override System.ServiceModel.Channels.BindingParameterCollection OnConfigureBinding(HttpBinding httpBinding)
        {
            if (_requiresSSL)
            {
                httpBinding.Security.Mode = HttpBindingSecurityMode.Transport;
            }

            var binding = base.OnConfigureBinding(httpBinding);
            return binding;
        }
    }
}