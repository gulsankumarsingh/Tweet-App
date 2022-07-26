﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace UserService.API.Infrastructure.Configuration.SwaggerConfig
{
    /// <summary>
    /// Class for Configuration of Swagger
    /// </summary>
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {

        /// <summary>
        /// Method for Configuring swagger options
        /// </summary>
        /// <param name="options">Swagger options parameter</param>
        public void Configure(SwaggerGenOptions options)
        {

            options.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = "User Service API",
                Version = "v1",
            });

            ////Adding support for JWT token in the Swagger
            //options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            //{
            //    Description =
            //   "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
            //   "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
            //   "Example: \"Bearer 12345abcdef\"",
            //    Name = "Authorization",
            //    In = ParameterLocation.Header,
            //    Type = SecuritySchemeType.ApiKey,
            //    Scheme = "Bearer"
            //});

            //options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            //    {
            //        {
            //            new OpenApiSecurityScheme
            //            {
            //                Reference = new OpenApiReference
            //                {
            //                    Type = ReferenceType.SecurityScheme,
            //                    Id = "Bearer"
            //                },
            //                Scheme = "oauth2",
            //                Name = "Bearer",
            //                In = ParameterLocation.Header,
            //            },
            //            new List<string>()
            //        }
            //    });

            //Adding comment file
            //var xmlCommentFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //var cmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentFile);
            //options.IncludeXmlComments(cmlCommentsFullPath);
        }
    }
}
