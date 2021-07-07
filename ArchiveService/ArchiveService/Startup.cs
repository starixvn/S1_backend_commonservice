using System;
using System.IO;
using ArchiveService.Service.BLL;
using ArchiveService.Service.DAL;
using ArchiveService.Workers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ArchiveService
{
    public class Startup
    {
        private static IConfigurationRoot Configuration => new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables()
            .AddJsonFile($"appsettings.json", false, true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", false, true)
            .Build();
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            /* BLL */
            services.AddSingleton<QueryTableBLL>();
            services.AddSingleton<InsertTable14BLL>();
            services.AddSingleton<DelOldItemBLL>();
            services.AddSingleton<ResetUsagedBLL>();
            

            /* DAL */
            services.AddSingleton<QueryTableService>();
            services.AddSingleton<InsertTable14Service>();
            services.AddSingleton<DelOldItemService>();
            services.AddSingleton<ResetUsagedService>();

            /* Worker */
            if (Configuration.GetSection("RunHistoryWorker").Get<bool>())
            {
                services.AddHostedService<HistoryWorker>();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
