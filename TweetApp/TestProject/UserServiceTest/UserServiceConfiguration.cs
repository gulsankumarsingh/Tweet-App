using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using User.API.Infrastructure.DataContext;
using UserService.API.Infrastructure.Configuration.AutoMapperConfig;

namespace UserServiceTest
{
    public class UserServiceConfiguration
    {
        private static IConfiguration _configuration = null;
        private static string _connectionString = null;
        private static UserDbContext _context;

        public static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }

        public static string GetConnectionString()
        {
            if (_configuration == null)
                _configuration = GetConfiguration();
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
            return _connectionString;
        }

        public static UserDbContext GetApplicationDbContext()
        {
            if (_configuration == null)
                _configuration = GetConfiguration();

            if (_connectionString == null)
                _connectionString = GetConnectionString();

            var dbContextOptions = new DbContextOptionsBuilder<UserDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            _context = new UserDbContext(dbContextOptions);
            return _context;
        }

        public static IMapper GetAutoMapperConfiguration()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ConfigureAutomapper());
            });
            return mappingConfig.CreateMapper();

        }
    }
}
