using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using User.API.Models;
using User.API.Models.Dtos;

namespace UserService.API.Infrastructure.Configuration.AutoMapperConfig
{
    public class ConfigureAutomapper: Profile
    {
        public ConfigureAutomapper()
        {
            CreateMap<UserProfile, CreateUserProfileDto>().ReverseMap();
            CreateMap<UserProfile, UpdateUserProfileDto>().ReverseMap();
            CreateMap<UserProfile, UserDto>().ReverseMap();
            CreateMap<UserProfile, UserProfileDto>().ReverseMap();
        }
    }
}
