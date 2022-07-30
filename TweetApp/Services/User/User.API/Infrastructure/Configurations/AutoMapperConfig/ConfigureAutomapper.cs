﻿namespace UserService.API.Infrastructure.Configuration.AutoMapperConfig
{
    using AutoMapper;
    using User.API.Models;
    using User.API.Models.Dtos;

    /// <summary>
    /// Class for configuring the AutoMapper
    /// </summary>
    public class ConfigureAutomapper : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureAutomapper"/> class.
        /// </summary>
        public ConfigureAutomapper()
        {
            CreateMap<UserProfile, CreateUserProfileDto>().ReverseMap();
            CreateMap<UserProfile, UpdateUserProfileDto>().ReverseMap();
            CreateMap<UserProfile, UserDto>().ReverseMap();
            CreateMap<UserProfile, UserProfileDto>().ReverseMap();
        }
    }
}
