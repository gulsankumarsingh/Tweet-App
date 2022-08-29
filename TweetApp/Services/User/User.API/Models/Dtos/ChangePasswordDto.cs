namespace User.API.Models.Dtos
{
    /// <summary>
    /// Defines the <see cref="ChangePasswordDto" />.
    /// </summary>
    public class ChangePasswordDto
    {
        /// <summary>
        /// Gets or sets the Password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the Email.
        /// </summary>
        public string Email { get; set; }
        public long ContactNumber { get; set; }
    }
}
