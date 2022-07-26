using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User.API.Models
{
    public class ApiResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public object ResponseValue { get; set; } = null;
    }
}
