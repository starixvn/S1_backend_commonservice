using ArchiveService.Service.BLL;
using ArchiveService.Service.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ArchiveService
{
    public class Startup
    {
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
