using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using HundredSDK.Authentication.BasicTokens;
using System.Security.Claims;
using System.Security.Principal;
using HundredSDK.Authentication.BasicTokens.TestWebApi.Model;
using HundredSDK.Authentication.BasicTokens.TestWebApi.Model.Services;
using Microsoft.Extensions.Caching.Memory;

namespace HundredSDK.Authentication.BasicTokens.TestWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            
            //IoC
            services.AddScoped<ISeguridadService, SeguridadService>();

            //Configurar Auth Token
            services.AddAuthenticationHundredBasicToken(this.Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            IMemoryCache cache)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //Configurar Middleware
            app.UseHundredTokenProvider(this.Configuration, cache);
            
            app.UseAuthentication();

            app.UseMvc();
            
        }
    }
}
