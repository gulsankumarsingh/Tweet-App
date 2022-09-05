using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using Prometheus;
using System.Text;
using Tweet.API.Infrastructure.Configuration.AutoMapperConfig;
using Tweet.API.Infrastructure.Configuration.SwaggerConfig;
using Tweet.API.Infrastructure.Configurations.RabbitMqConfig;
using Tweet.API.Infrastructure.DataContext;
using Tweet.API.Infrastructure.Filters;
using Tweet.API.Infrastructure.Repository;
using Tweet.API.Infrastructure.Repository.Interface;
using Tweet.API.Infrastructure.Services.MessageConsumerService;
using Tweet.API.Infrastructure.Services.AzureServiceBusConsumer;
using Tweet.API.Infrastructure.Configurations.AzureBusConfig;

namespace Tweet.API
{
    /// <summary>
    /// Defines the <see cref="Startup" />.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration<see cref="IConfiguration"/>.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the Configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        /// <summary>
        /// The ConfigureServices.
        /// </summary>
        /// <param name="services">The services<see cref="IServiceCollection"/>.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddOptions();

            var rabbitMqConfig = Configuration.GetSection("RabbitMq");
            services.Configure<RabbitMqConfiguration>(rabbitMqConfig);

            var azureBusConfig = Configuration.GetSection("AzureBusConfiguration");
            services.Configure<AzureBusConfiguration>(azureBusConfig);

            services.AddControllers(options => options.Filters.Add(typeof(HttpGlobalExceptionFilter)));

            //Comment below line in case of local run
            services.AddDbContext<TweetDbContext>(options =>
                options.UseCosmos(Configuration["CosmosConfiguration:CosmosEndPoint"],
                    Configuration["CosmosConfiguration:CosmosKey"],
                    Configuration["CosmosConfiguration:Database"]));

            //Uncomment below line in case of local run
            //services.AddDbContext<TweetDbContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<ITweetRepository, TweetRepository>();

            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(options => options.GroupNameFormat = "'v'VVV");
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen();
            services.AddAutoMapper(typeof(ConfigureAutomapper));

            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("JwtDetail").GetSection("Key").Value)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = Configuration.GetSection("JwtDetail").GetSection("Issuer").Value,
                    ValidAudience = Configuration.GetSection("JwtDetail").GetSection("Audience").Value,
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = true
                };
            });

            services.AddCors(options => 
            {
                options.AddPolicy("CorsPolicy", builder => builder
                .SetIsOriginAllowed((host) => true)
                .AllowAnyMethod()
                .AllowCredentials()
                .AllowAnyHeader()
                );
            });

            //services.AddHostedService<DeleteUserMessageConsumer>();
            services.AddHostedService<DeleteUserConsumer>();

            services.AddHealthChecks()
                .AddSqlServer(Configuration.GetConnectionString("DefaultConnection"), null, "Sql Server Health Check", HealthStatus.Unhealthy)
                .AddRabbitMQ($"amqp://{Configuration["RabbitMq:HostName"]}:5672", null, "RabbitMq Health Check", HealthStatus.Unhealthy);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// The Configure.
        /// </summary>
        /// <param name="app">The app<see cref="IApplicationBuilder"/>.</param>
        /// <param name="env">The env<see cref="IWebHostEnvironment"/>.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TweetService.API v1"));
            
            app.UseRouting();
            app.UseHttpMetrics();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/healthcheck", new HealthCheckOptions() {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapMetrics();
            });
        }
    }
}
