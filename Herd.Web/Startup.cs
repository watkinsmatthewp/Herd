using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Herd.Business;
using Herd.Data.Models;
using Herd.Business.Models.Commands;

namespace Herd_Web
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }
            else
            {
                //app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });

            OnRun();
        }

        private void OnRun()
        {
            HerdApp.Instance.CreateUser(new HerdAppCreateUserCommand
            {
                Email = "mpwatki2@ncsu.edu",
                FirstName = "Matthew",
                LastName = "Watkins",
                PasswordPlainText = "password"
            });
            HerdApp.Instance.CreateUser(new HerdAppCreateUserCommand
            {
                Email = "tdortiz@ncsu.edu",
                FirstName = "Thomas",
                LastName = "Ortiz",
                PasswordPlainText = "password"
            });
            HerdApp.Instance.CreateUser(new HerdAppCreateUserCommand
            {
                Email = "dbchris3@ncsu.edu",
                FirstName = "Dana",
                LastName = "Chritso",
                PasswordPlainText = "password"
            });
            HerdApp.Instance.CreateUser(new HerdAppCreateUserCommand
            {
                Email = "jcstone3@ncsu.edu",
                FirstName = "Jacob",
                LastName = "Stone",
                PasswordPlainText = "password"
            });
        }
    }
}
