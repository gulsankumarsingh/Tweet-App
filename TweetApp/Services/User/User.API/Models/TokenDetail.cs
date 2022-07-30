namespace User.API.Models
{
    using System;

    /// <summary>
    /// Defines the <see cref="TokenDetail" />.
    /// </summary>
    public class TokenDetail
    {
        /// <summary>
        /// Gets or sets the Token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the Expiration.
        /// </summary>
        public DateTime Expiration { get; set; }
    }
}
