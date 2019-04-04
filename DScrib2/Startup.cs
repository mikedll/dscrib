using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DScrib2.Models;
using System.IO;

namespace DScrib2
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: false, reloadOnChange: true)
                .Build();

            var dbConf = config["Data:connectionString"];

            services
                .AddEntityFrameworkNpgsql()
                .AddDbContext<AppDbContext>(options =>
                options
                    .UseLazyLoadingProxies()
                    .UseNpgsql(dbConf)    
            );
                            
            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN-CUSTOM";
            });

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
            });

            services.AddMvc(options =>
            {
                options.Filters.Add(new HandleGSecretFilter());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSession();
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "Search",
                    template: "search",
                    defaults: new { controller = "Reviews", action = "Search" }
                );

                routes.MapRoute(
                    name: "Review",
                    template: "reviews/fetch",
                    defaults: new { controller = "Reviews", action = "Fetch" }
                );

                routes.MapRoute(
                    name: "Creates",
                    template: "{controller}",
                    defaults: new { action = "Create" }
                );

                routes.MapRoute(
                    name: "default", template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
