namespace Tweet.API.Infrastructure.Configuration.AutoMapperConfig
{
    using AutoMapper;
    using Tweet.API.Models;
    using Tweet.API.Models.Dtos;

    /// <summary>
    /// Class for configuration of AutoMapper
    /// </summary>
    public class ConfigureAutomapper : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureAutomapper"/> class.
        /// </summary>
        public ConfigureAutomapper()
        {
            CreateMap<TweetDetail, TweetDto>().ReverseMap();
            CreateMap<Comment, CommentDto>().ReverseMap();
        }
    }
}
