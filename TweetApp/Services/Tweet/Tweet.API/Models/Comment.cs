using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tweet.API.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(144, ErrorMessage = "Only upto 144 characters allowed.")]
        public string Message { get; set; }
        [Required]
        public DateTime CommentAt { get; set; } = DateTime.UtcNow;
        [Required]
        public int TweetId { get; set; }
        [Required]
        public string UserName { get; set; }
    }
}
