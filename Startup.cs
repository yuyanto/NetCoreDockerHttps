using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using WhoGetWhat.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace WhoGetWhat
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
            var hostname = Environment.GetEnvironmentVariable("SQLSERVER_HOST") ?? "localhost";
            services.AddControllersWithViews(o => o.Filters.Add(new AuthorizeFilter()));
            //services.AddRazorPages().AddMvcOptions(o => o.Filters.Add(new AuthorizeFilter()));
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });


            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            //services.AddScoped<IPasswordHasher<Model.user>, PasswordHasher<Model.user>>();

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie("SchemeName");

            
            services.AddControllersWithViews().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
            services.AddRazorPages();
        //    services.AddAuthentication()
        //.AddGoogle(options =>
        //{
        //    IConfigurationSection googleAuthNSection =
        //        Configuration.GetSection("Authentication:Google");

        //    options.ClientId = googleAuthNSection["ClientId"];
        //    options.ClientSecret = googleAuthNSection["ClientSecret"];
        //});

            //services.AddAntiforgery(options =>
            //{
            //    // Set Cookie properties using CookieBuilder properties�.
            //    options.FormFieldName = "AntiforgeryFieldname";
            //    options.HeaderName = "X-CSRF-TOKEN-HEADERNAME";
            //    options.SuppressXFrameOptionsHeader = false;
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IAntiforgery antiforgery)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            //app.Use(next => context =>
            //{
            //    string path = context.Request.Path.Value;

            //    if (
            //        string.Equals(path, "/", StringComparison.OrdinalIgnoreCase) ||
            //        string.Equals(path, "/index.html", StringComparison.OrdinalIgnoreCase))
            //    {
            //        // The request token can be sent as a JavaScript-readable cookie, 
            //        // and Angular uses it by default.
            //        var tokens = antiforgery.GetAndStoreTokens(context);
            //        context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken,
            //            new CookieOptions() { HttpOnly = false });
            //    }

            //    return next(context);
            //});
        }
    }
}
