using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Autofac;
using CSRedis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Qf.Core;
using Qf.Core.Tasks;
using Qf.Core.Web.Authentication.WeChat;
using Qf.Core.Web.Authorization;
using Qf.Core.Web.Extension;
using Qf.Core.Web.Filters;

namespace Qf.SysTodoList.WebApi
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

        /// <summary>
        /// Gets 配置项
        /// </summary>
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCustomMvc(Configuration)
                .AddHealthChecks(Configuration)
                .AddCustomDbContext(Configuration)
                .AddHttpClient()
                .AddMemoryCache()
                .AddRedisCache(Configuration)
                .AddCustomSwagger(Configuration)
                .AddCustomConfiguration(Configuration);
            services.AddAuthentication(BearerAuthorizeAttribute.DefaultAuthenticationScheme)
                .AddCookie(BearerAuthorizeAttribute.DefaultAuthenticationScheme, o =>
                {
                    o.Cookie.Name = BearerAuthorizeAttribute.DefaultAuthenticationScheme;
                    o.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                }).AddWeChat(wechatOptions =>
                {
                    wechatOptions.CallbackPath = new PathString($"/{Program.AppName}/signin-wechat");
                    wechatOptions.CallbackUrl = $"{Configuration["WeChat:CallbackPath"]}/{Program.AppName}/signin-wechat";
                    wechatOptions.AppId = Configuration["WeChat:AppId"];
                    wechatOptions.AppSecret = Configuration["WeChat:AppSecret"];
                    wechatOptions.UseCachedStateDataFormat = true;
                });
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

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(Startup.PolicyName);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwagger(c => { c.RouteTemplate = "doc/{documentName}/swagger.json"; })
               .UseSwaggerUI(c =>
               {
                   c.SwaggerEndpoint($"/doc/{Program.AppName}/swagger.json", $"{Program.AppName} {Configuration["Version"]}");
               });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true
                });
                endpoints.MapControllers();
            });
        }
    }
    /// <summary>
    /// 自定义扩展
    /// </summary>
    public static class CustomExtensionsMethods
    {
        public static IServiceCollection AddCustomMvc(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(WebApiResultMiddleware));
            })
            //.AddNewtonsoftJson();//添加基于 Newtonsoft.Json 的 JSON 格式支持
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            });
            services.AddCors(options =>
            {
                var origs = configuration.GetSection("AllowOrigins").Get<string[]>();
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
            return services;
        }

        public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            var hcBuilder = services.AddHealthChecks();

            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());
            hcBuilder.AddSqlServer(configuration["ConnectionString"]);

            return services;
        }
        public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            var redisDB = new CSRedisClient(configuration["Redis"]);
            RedisHelper.Initialization(redisDB);
            services.AddSingleton<IDistributedCache>(new CSRedisCache(redisDB));
            services.AddSingleton(redisDB);
            return services;
        }
        public static IServiceCollection AddCustomConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddHostedService<QueuedHostedService>();

            services.AddOptions();
            services.Configure<AppSettings>(configuration);
            return services;
        }
        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {

            return services;
        }
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(Program.AppName, new OpenApiInfo
                {
                    Title = configuration["Title"],
                    Version = configuration["Version"],
                    Description = configuration["Description"]
                });

                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
            });

            return services;
        }
    }
}
