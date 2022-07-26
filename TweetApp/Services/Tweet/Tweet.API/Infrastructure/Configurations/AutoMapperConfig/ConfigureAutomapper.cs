using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet.API.Models;
using Tweet.API.Models.Dtos;

namespace Tweet.API.Infrastructure.Configuration.AutoMapperConfig
{
    public class ConfigureAutomapper: Profile
    {
        public ConfigureAutomapper()
        {
            CreateMap<TweetDetail, TweetDto>().ReverseMap();
            CreateMap<Comment, CommentDto>().ReverseMap();
        }
    }
}
