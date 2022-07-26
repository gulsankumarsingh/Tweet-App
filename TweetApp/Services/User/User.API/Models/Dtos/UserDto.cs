using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User.API.Models.Dtos
{
    public class UserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string LoginId { get; set; }
        public string Status { get; set; }
        public string ProfileImg { get; set; }

    }
}
