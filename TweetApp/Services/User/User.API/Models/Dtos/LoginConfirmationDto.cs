namespace User.API.Models.Dtos
{
    /// <summary>
    /// Defines the <see cref="LoginConfirmationDto" />.
    /// </summary>
    public class LoginConfirmationDto
    {
        /// <summary>
        /// Gets or sets the TokenDetail.
        /// </summary>
        public TokenDetail TokenDetail { get; set; }

        /// <summary>
        /// Gets or sets the UserInfo.
        /// </summary>
        public UserDto UserInfo { get; set; }
    }
}
