using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tweet.API.Models.Dtos
{
    public class CommentDto
    {
        [Required]
        [MaxLength(144, ErrorMessage = "Only upto 144 characters allowed.")]
        public string Message { get; set; }
        [Required]
        public int TweetId { get; set; }
        [Required]
        public string UserName { get; set; }
    }
}
