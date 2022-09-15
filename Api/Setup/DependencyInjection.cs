using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infra.Crosscutting.Logs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Setup
{
    public static class DependencyInjection
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ILogModel, LogModel>();

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
