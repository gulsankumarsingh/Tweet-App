using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tweet.API.Models.Dtos
{
    public class UpdateReplyDto
    {
        [Required]
        public string Id { get; set; }
        [Required]
        [MaxLength(144, ErrorMessage = "Only upto 144 characters allowed.")]
        public string Message { get; set; }
    }
}
