using Microsoft.AspNetCore.Http;
using System;

namespace User.API.Models.Dtos
{
    /// <summary>
    /// Defines the <see cref="UserDto" />.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Gets or sets the FullName.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the Username.
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Gets or sets the Email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the Status.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the ProfileImg.
        /// </summary>
        public string ProfileImage { get; set; }
        /// <summary>
        /// Gets or sets the IsActive.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the LogoutAt.
        /// </summary>
        public DateTime LogoutAt { get; set; }
    }
}
