using Herd.Business;
using Herd.Business.Models.Commands;
using Herd.Data.Providers;
using Herd.Logging;
using Herd.Web.Code;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Herd.Web
{
    public class Startup
    {
        public const string COOKIE_AUTH_SCHEME_NAME = "HerdCookieAuthenticationScheme";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddAuthentication(COOKIE_AUTH_SCHEME_NAME)
            .AddCookie(COOKIE_AUTH_SCHEME_NAME, options =>
            {
                // options.AccessDeniedPath = "/Account/Forbidden/";
                options.LoginPath = "/login";
                options.Cookie.Name = "HERD_SESSION";
                options.Cookie.HttpOnly = false;
                options.Cookie.Expiration = TimeSpan.FromDays(15);
                options.SlidingExpiration = true;
                options.Cookie.Path = "/";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            RegisterErrorHandler(app);

            if (env.IsDevelopment())
            {
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }

            app.UseStaticFiles();
            app.UseAuthentication();

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

        private static void RegisterErrorHandler(IApplicationBuilder app)
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var errorID = Guid.NewGuid();
                    var error = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                    Console.WriteLine($"ERROR {errorID}: {error}");
                    try
                    {
                        HerdWebApp.Instance.Logger.Error(errorID, "Error processing request", new Dictionary<string, string>
                        {
                            ["PATH"] = context.Request.Path,
                            ["HEDERS"] = FormatHeaders(context.Request.Headers),
                            ["BODY"] = await new StreamReader(context.Request.Body ?? new MemoryStream()).ReadToEndAsync(),
                        }, error);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    context.Response.StatusCode = 500;
                    var enpdoint = context.Request.ContentType?.Contains("json") == true ? "/json" : "";
                    context.Response.Redirect($"/error/{enpdoint}?id={errorID}");
                });
            });
        }

        private static string FormatHeaders(IHeaderDictionary headers)
        {
            var sb = new StringBuilder();
            foreach (var headerGroup in headers)
            {
                sb.Append($"{headerGroup.Key}={string.Join(",", headerGroup.Value)};");
            }
            return sb.ToString();
        }

        private void OnRun()
        {
            var app = new HerdApp(new HerdFileDataProvider(), new MastodonApiWrapper("mastodon.xyz"), HerdWebApp.Instance.Logger);

            try
            {
                app.CreateUser(new CreateUserCommand
                {
                    Email = "mpwatki2@ncsu.edu",
                    FirstName = "Matthew",
                    LastName = "Watkins",
                    PasswordPlainText = "password"
                });
            }
            catch { }

            try
            {
                app.CreateUser(new CreateUserCommand
                {
                    Email = "tdortiz@ncsu.edu",
                    FirstName = "Thomas",
                    LastName = "Ortiz",
                    PasswordPlainText = "password"
                });
            }
            catch { }

            try
            {
                app.CreateUser(new CreateUserCommand
                {
                    Email = "dbchris3@ncsu.edu",
                    FirstName = "Dana",
                    LastName = "Chritso",
                    PasswordPlainText = "password"
                });
            }
            catch { }

            try
            {
                app.CreateUser(new CreateUserCommand
                {
                    Email = "jcstone3@ncsu.edu",
                    FirstName = "Jacob",
                    LastName = "Stone",
                    PasswordPlainText = "password"
                });
            }
            catch { }
        }
    }
}