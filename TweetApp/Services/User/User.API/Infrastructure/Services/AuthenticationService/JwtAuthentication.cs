namespace User.API.Infrastructure.Services.AuthenticationService
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using User.API.Infrastructure.Services.AuthenticationService.Interfaces;
    using User.API.Models;

    /// <summary>
    /// Defines the <see cref="JwtAuthentication" />.
    /// </summary>
    public class JwtAuthentication : IJwtAuthentication
    {
        /// <summary>
        /// Defines the _configuration.
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Defines the _logger.
        /// </summary>
        private readonly ILogger<JwtAuthentication> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtAuthentication"/> class.
        /// </summary>
        /// <param name="configuration">The configuration<see cref="IConfiguration"/>.</param>
        /// <param name="logger">The logger<see cref="ILogger{JwtAuthentication}"/>.</param>
        public JwtAuthentication(IConfiguration configuration, ILogger<JwtAuthentication> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Method for Generating the JWT Token.
        /// </summary>
        /// <param name="username">The username<see cref="string"/>.</param>
        /// <returns>The token detail</returns>
        public TokenDetail GenerateToken(string username)
        {
            TokenDetail tokenDetail = null;
            try
            {
                var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration.GetSection("JwtDetail").GetSection("Subject").Value),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim(ClaimTypes.Name, username)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JwtDetail").GetSection("Key").Value));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

                var token = new JwtSecurityToken(
                        _configuration.GetSection("JwtDetail").GetSection("Issuer").Value,
                            _configuration.GetSection("JwtDetail").GetSection("Audience").Value,
                        claims,
                        expires: DateTime.UtcNow.AddDays(7),
                        signingCredentials: signIn);
                var tokenHandler = new JwtSecurityTokenHandler();

                tokenDetail = new TokenDetail
                {
                    Username= username,
                    Token = tokenHandler.WriteToken(token),
                    Expiration = token.ValidTo
                };
            }
            catch (Exception ex)
            {

                _logger.LogError("An error occured while generating the JWT token", ex.Message);
            }
            return tokenDetail;
        }
    }
}
