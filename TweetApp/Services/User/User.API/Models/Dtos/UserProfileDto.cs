namespace User.API.Models.Dtos
{
    using System;

    /// <summary>
    /// Defines the <see cref="UserProfileDto" />.
    /// </summary>
    public class UserProfileDto
    {
        /// <summary>
        /// Gets or sets the Email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the LoginId.
        /// </summary>
        public string LoginId { get; set; }

        /// <summary>
        /// Gets or sets the FirstName.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the LastName.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the Gender.
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// Gets or sets the DateOfBirth.
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the ContactNumber.
        /// </summary>
        public long ContactNumber { get; set; }

        /// <summary>
        /// Gets or sets the ProfileImg.
        /// </summary>
        public string ProfileImg { get; set; }

        /// <summary>
        /// Gets or sets the Status.
        /// </summary>
        public string Status { get; set; }
    }
}
