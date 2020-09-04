using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;
using Qf.Core.Web.Extension;
using System.Collections.Generic;

namespace Qf.APIGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        /// <summary>
        /// Gets or sets 跨域策略
        /// </summary>
        public static string PolicyName { get; set; } = "allow_all";
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                var origs = Configuration.GetSection("AllowOrigins").Get<string[]>();
                if (origs != null)
                {
                    Startup.PolicyName = "with_origins";
                    options.AddPolicy(Startup.PolicyName, builder =>
                    {
                        builder.SetIsOriginAllowedToAllowWildcardSubdomains()
                        .WithOrigins(origs)
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                    });
                }

                options.AddPolicy("allow_all", builder =>
                {
                    builder.SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyOrigin() // 允许任何来源的主机访问
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                    // .AllowCredentials();//指定处理cookie
                });
            });
            services.AddOcelot().AddPolly();
        }
        /// <summary>
        /// ConfigureContainer is where you can register things directly with Autofac. This runs after ConfigureServices so the things
        /// here will override registrations made in ConfigureServices.
        /// Don't build the container; that gets done for you. If you
        /// need a reference to the container, you need to use the
        /// "Without ConfigureContainer" mechanism shown later.
        /// </summary>
        public void ConfigureContainer(ContainerBuilder builder)
        {
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = new FileExtensionContentTypeProvider(new Dictionary<string, string>
                    {
                      { ".apk","application/vnd.android.package-archive"},
                      { ".nupkg","application/zip"}
                    }),
                ServeUnknownFileTypes = true//设置不限制content-type
            });
            app.UseCors(Startup.PolicyName);
            app.UseOcelot().Wait();
        }
    }
}
