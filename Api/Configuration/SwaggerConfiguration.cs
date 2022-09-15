using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Api.Setup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Api.Configuration
{
    [ExcludeFromCodeCoverage]
    public static class SwaggerConfiguration
    {
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services
                .AddMvc(options =>
                {
                    options.InputFormatters.RemoveType<Microsoft.AspNetCore.Mvc.Formatters.SystemTextJsonInputFormatter>();
                    options.OutputFormatters.RemoveType<Microsoft.AspNetCore.Mvc.Formatters.SystemTextJsonOutputFormatter>();
                })
                .AddNewtonsoftJson(opts =>
                {
                    opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    opts.SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
                })
                .AddXmlSerializerFormatters();


            services.AddSwaggerGen(c =>
            {

                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "Api OpenBanking Admin",
                        Version = ApiVersion.Get(),
                        Description = string.Format("Environment: {0} - Server Time: {1}", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), DateTime.UtcNow.ToString("o")),
                    });
            });
            return services;
        }


        public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env != null && env.EnvironmentName == "Production")
                return app;

            app.UseSwagger(c =>
            {
                c.RouteTemplate = "admin/swagger/{documentName}/swagger.json";
                c.PreSerializeFilters.Add((swagger, httpReq) =>
                {
                    string serverUrl = $"https://{httpReq.Host.Host}";
                    if (httpReq.Host.Port != null)
                        serverUrl += $":{httpReq.Host.Port}";
                    swagger.Servers = new List<OpenApiServer> { new OpenApiServer { Url = serverUrl } };
                });
            });
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "admin/swagger";
                c.SwaggerEndpoint("/admin/swagger/v1/swagger.json", "API OpenBanking Admin");
            });

            return app;
        }
    }
}
