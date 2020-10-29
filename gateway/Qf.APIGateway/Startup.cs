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
        /// Gets or sets �������
        /// </summary>
        public static string PolicyName { get; set; } = "allow_all";
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //���HttpReports�ʹ洢
            services.AddHttpReports().UseHttpTransport();
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
                    .AllowAnyOrigin() // �����κ���Դ����������
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                    // .AllowCredentials();//ָ������cookie
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
            //ʹ��HttpReports�м��
            app.UseHttpReports();
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = new FileExtensionContentTypeProvider(new Dictionary<string, string>
                    {
                      { ".apk","application/vnd.android.package-archive"},
                      { ".nupkg","application/zip"}
                    }),
                ServeUnknownFileTypes = true//���ò�����content-type
            });
            app.UseCors(Startup.PolicyName);
            app.UseOcelot().Wait();
        }
    }
}
