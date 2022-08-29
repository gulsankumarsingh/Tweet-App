using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tweet.API.Infrastructure.Configuration.AutoMapperConfig;
using Tweet.API.Infrastructure.DataContext;

namespace TweetServiceTest
{
    public class TweetServiceConfiguration
    {
        private static IConfiguration _configuration = null;
        private static string _connectionString = null;
        private static TweetDbContext _context;

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

        public static TweetDbContext GetApplicationDbContext()
        {
            if (_configuration == null)
                _configuration = GetConfiguration();

            if (_connectionString == null)
                _connectionString = GetConnectionString();

            var dbContextOptions = new DbContextOptionsBuilder<TweetDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            _context = new TweetDbContext(dbContextOptions);
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
