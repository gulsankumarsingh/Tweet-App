using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User.API.Models.Dtos
{
    public class ProfileImageDto
    {
        public IFormFile ProfileImageFile { get; set; }
    }
}
