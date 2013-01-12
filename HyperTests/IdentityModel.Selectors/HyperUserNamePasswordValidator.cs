using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;

namespace HyperTests.IdentityModel.Selectors
{
    /// <summary>
    /// HyperUserNamePasswordValidator class.
    /// </summary>
    public class HyperUserNamePasswordValidator : UserNamePasswordValidator
    {
        /// <summary>
        /// When overridden in a derived class, validates the specified username and password.
        /// </summary>
        /// <param name="userName">The username to validate.</param>
        /// <param name="password">The password to validate.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IdentityModel.Tokens.SecurityTokenException"></exception>
        public override void Validate(string userName, string password)
        {
            if (null == userName || null == password)
            {
                throw new ArgumentNullException();
            }

            if (!(userName == "username" && password == "password"))
            {
                throw new SecurityTokenException("Unknown Username or Password");
            }
        }
    }
}
