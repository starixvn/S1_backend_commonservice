using System;
using System.IO;
using ArchiveService.Workers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;

namespace ArchiveService
{
    public class Program
    {
        private static IConfigurationRoot Configuration => new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables()
            .AddJsonFile($"appsettings.json", false, true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", false, true)
            .Build();
        
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseSerilog((hostContext, loggerConfiguration) =>
                    {
                        loggerConfiguration
                            .ReadFrom.Configuration(Configuration)
                            .Enrich.FromLogContext()
                            .Enrich.WithExceptionDetails();
                    })
                    .UseStartup<Startup>();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    if (Configuration.GetSection("RunHistoryWorker").Get<bool>())
                    {
                        services.AddHostedService<HistoryWorker>();
                    }
                });
    }
}
