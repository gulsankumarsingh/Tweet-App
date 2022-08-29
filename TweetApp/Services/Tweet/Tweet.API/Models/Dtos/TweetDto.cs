using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tweet.API.Models.Dtos
{
    public class TweetDto
    {
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the Message.
        /// </summary>
        [Required]
        [MaxLength(144, ErrorMessage = "Only upto 144 characters allowed.")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the CreatedAt.
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        /// <summary>
        /// Gets or sets the CreatedAt.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the UserName.
        /// </summary>
        [Required]
        public string UserName { get; set; }
    }
}
