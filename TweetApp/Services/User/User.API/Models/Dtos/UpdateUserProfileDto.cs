using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace User.API.Models.Dtos
{
    public class UpdateUserProfileDto
    {
        [Required]
        public string LoginId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public long ContactNumber { get; set; }
        public string Status { get; set; }
        public string ProfileImg { get; set; }
    }
}
