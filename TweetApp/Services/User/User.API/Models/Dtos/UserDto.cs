namespace User.API.Models.Dtos
{
    /// <summary>
    /// Defines the <see cref="UserDto" />.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Gets or sets the FirstName.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the LastName.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the LoginId.
        /// </summary>
        public string LoginId { get; set; }

        /// <summary>
        /// Gets or sets the Status.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the ProfileImg.
        /// </summary>
        public string ProfileImg { get; set; }
    }
}
