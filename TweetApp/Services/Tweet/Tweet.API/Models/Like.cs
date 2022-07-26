using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tweet.API.Models
{
    public class Like
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int TweetId { get; set; }
        [Required]
        public string UserName { get; set; }
    }
}
