using Api.Configuration;
using Api.Setup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                }).AddDataAnnotationsLocalization();


            services.AddHealthCheckConfiguration(Configuration);
            services.AddSwaggerConfiguration();
            services.AddCorsConfiguration(Configuration["Cors:Authorize"]);

            services.RegisterServices(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.EnvironmentName == "Dev")
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHealthCheckConfiguration();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseSwaggerConfiguration(env);
            app.UseCorsConfiguration();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
