using Microsoft.Extensions.DependencyInjection;
using Shiny;
using System;
using System.Collections.Generic;
using System.Text;

namespace XFUserForegroundService
{
    public class AppShinyStartup : ShinyStartup
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.UseGps();
        }
    }
}
