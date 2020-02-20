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

namespace BR904WIP
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
            services.AddCors();
            Environment.SetEnvironmentVariable("BR_DB_HOST_NAME", Configuration["BR_DB_HOST_NAME"]);
            Environment.SetEnvironmentVariable("BR_DB_USER_NAME", Configuration["BR_DB_USER_NAME"]);
            Environment.SetEnvironmentVariable("BR_DB_PASSWORD", Configuration["BR_DB_PASSWORD"]);
            Environment.SetEnvironmentVariable("BR_DB_PORT", Configuration["BR_DB_PORT"]);
            Environment.SetEnvironmentVariable("BR_DB_NAME", Configuration["BR_DB_NAME"]);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(builder => builder
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin()
            .AllowCredentials()
            );
            app.UseMvc();
        }
    }
}
