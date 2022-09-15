using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Api.Configuration
{
    [ExcludeFromCodeCoverage]
    public static class CorsConfiguration
    {
        public static readonly string CorsName = "CORSPOLICY";

        public static IServiceCollection AddCorsConfiguration(this IServiceCollection services, string origins)
        {
            return services.AddCors(options =>
            {
                options.AddPolicy(CorsName, builder => builder.WithOrigins(origins.Split(';'))
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithExposedHeaders(new List<string>() { "X-Pagination", "X-Summary", "Content-Disposition" }.ToArray()));
            });
        }

        public static IApplicationBuilder UseCorsConfiguration(this IApplicationBuilder app)
        {
            return app.UseCors(CorsName);
        }
    }
}
