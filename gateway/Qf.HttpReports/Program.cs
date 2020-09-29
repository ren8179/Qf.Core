using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Qf.Core.Autofac;
using Qf.Core.Helper;
using Serilog;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Qf.HttpReports
{
    public class Program
    {
        /// <summary>
        /// Gets 服务名称
        /// </summary>
        public const string AppName = "apihttpreports";

        /// <summary>
        /// Gets or sets ip
        /// </summary>
        public static string IP { get; set; }

        /// <summary>
        /// Gets or sets 端口
        /// </summary>
        public static int Port { get; set; }

        public static async Task<int> Main(string[] args)
        {
            var isService = !(Debugger.IsAttached || args.Contains("--console"));
            var config = GetConfiguration();
            IP = config["IP"];
            Port = Convert.ToInt32(config["Port"]);
            Log.Logger = CreateSerilogLogger(config);
            if (string.IsNullOrEmpty(IP)) IP = NetworkHelper.LocalIPAddress;
            if (Port == 0) Port = NetworkHelper.GetRandomAvaliablePort();
            Log.Debug("Configuring host ({Application}) Begin Run host {IP}:{Port} ...", AppName, IP, Port);
            var host = CreateHostBuilder(args).Build();
            try
            {
                Log.Logger.Information("Starting {Application}({version}) {Service} {url} ", AppName, config["Version"], isService ? "win service" : "web host", $"http://{IP}:{Port}/");
                await host.RunAsync();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Program terminated unexpectedly ({Application})!", AppName);
                await host.StopAsync();
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .UseServiceProviderFactory(new AutofacServiceProviderFactory())
               .ConfigureAppConfiguration((hostingContext, config) =>
               {
                   config
                       .AddJsonFile("appsettings.json", true, true)
                       .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                       .AddEnvironmentVariables();
               })
               .UseSerilog()
               .ConfigureWebHostDefaults(webBuilder =>
               {
                   webBuilder
                   .UseStartup<Startup>()
                   .UseUrls($"http://{IP}:{Port}");
               });

        private static Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            return new LoggerConfiguration()
                .Enrich.WithProperty("Application", AppName)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            return builder.Build();
        }
    }
}
