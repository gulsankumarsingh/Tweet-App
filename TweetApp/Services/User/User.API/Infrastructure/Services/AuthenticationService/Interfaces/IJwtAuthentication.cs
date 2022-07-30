namespace User.API.Infrastructure.Services.AuthenticationService.Interfaces
{
    using User.API.Models;

    /// <summary>
    /// Defines the <see cref="IJwtAuthentication" />.
    /// </summary>
    public interface IJwtAuthentication
    {
        /// <summary>
        /// Method for Generate JWT Token.
        /// </summary>
        /// <param name="username">The username<see cref="string"/>.</param>
        /// <returns>The <see cref="TokenDetail"/>.</returns>
        public TokenDetail GenerateToken(string username);
    }
}
