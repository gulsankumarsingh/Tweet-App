namespace User.API.Models
{
    using Microsoft.AspNetCore.Http;
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Defines the <see cref="UserProfile" />.
    /// </summary>
    public class UserProfile
    {
        /// <summary>
        /// Gets or sets the Username.
        /// </summary>
        [Key]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the Email.
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the FirstName.
        /// </summary>
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the LastName.
        /// </summary>
        [Required]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the Gender.
        /// </summary>
        [Required]
        public string Gender { get; set; }

        /// <summary>
        /// Gets or sets the DateOfBirth.
        /// </summary>
        [Required]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the Password.
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the ContactNumber.
        /// </summary>
        [Required]
        public long ContactNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsActive.
        /// </summary>
        public bool IsActive { get; set; } = false;

        /// <summary>
        /// Gets or sets the LogoutAt.
        /// </summary>
        public DateTime LogoutAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the ProfileImg.
        /// </summary>
        public string ProfileImage { get; set; }
        /// <summary>
        /// Gets or sets the ProfileImg.
        /// </summary>
        public string ImageName { get; set; }

        /// <summary>
        /// Gets or sets the Status.
        /// </summary>
        public string Status { get; set; }
    }
}
