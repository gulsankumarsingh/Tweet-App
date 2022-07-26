using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tweet.API.Models
{
    public class TweetDetail
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(144, ErrorMessage = "Only upto 144 characters allowed.")]
        public string Message { get; set; }
        [Required]
        public DateTime TweetTime { get; set; } = DateTime.Now;
        [Required]
        public string UserName { get; set; }
    }
}
