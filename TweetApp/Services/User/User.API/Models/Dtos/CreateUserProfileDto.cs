using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace User.API.Models.Dtos
{
    public class CreateUserProfileDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string LoginId { get; set; }
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
    }
}
