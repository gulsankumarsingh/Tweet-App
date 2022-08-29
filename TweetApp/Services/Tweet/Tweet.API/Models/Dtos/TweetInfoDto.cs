namespace Tweet.API.Models.Dtos
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Defines the <see cref="TweetInfoDto" />.
    /// </summary>
    public class TweetInfoDto
    {
        public TweetDto Tweet { get; set; }

        public List<LikeDto> Likes { get; set; }
        public List<ReplyDto> Comments { get; set; }
    }
}
