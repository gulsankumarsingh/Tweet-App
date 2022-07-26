using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace User.API.Models
{
    public class UserProfile
    {
        [Key]
        public string LoginId { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public long ContactNumber { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime LogoutAt { get; set; }
        public string ProfileImg { get; set; }
        public string Status { get; set; }
    }
}
