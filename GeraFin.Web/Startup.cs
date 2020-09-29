using System;
using GeraFin.Services;
using GeraFin.DAL.DataAccess;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using GeraFin.InterFaces.Factory;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using GeraFin.Models.ViewModels.GeraFinWeb;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace GeraFin
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        [Obsolete]
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DevEnvR")));

            IConfigurationSection identityDefaultOptionsConfigurationSection = Configuration.GetSection("IdentityDefaultOptions");

            var identityDefaultOptions = identityDefaultOptionsConfigurationSection.Get<IdentityDefaultOptions>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)

                .AddCookie(options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.CookieName = ".MatimbaSession";
                    options.Cookie.IsEssential = true;
                    options.Cookie.Expiration = TimeSpan.FromDays(7);
                    options.ExpireTimeSpan = TimeSpan.FromDays(7);
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/SignedOut";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.SlidingExpiration = true;
                });

            services.Configure<CookiePolicyOptions>((Action<CookiePolicyOptions>)(options =>
            {
                options.CheckConsentNeeded = (Func<HttpContext, bool>)(context => true);
                options.MinimumSameSitePolicy = SameSiteMode.None;
            }));

            services.AddDistributedMemoryCache();
            
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(365);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            }); 
            
            services.AddHttpContextAccessor();

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredUniqueChars = 1;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.AllowedForNewUsers = true;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddMemoryCache();
            //services.Configure<IdentityDefaultOptions>(identityDefaultOptionsConfigurationSection);
            services.AddResponseCompression();
            services.AddSingleton<IEmailSender, EmailSender>();
            services.Configure<SendGridOptions>(Configuration.GetSection("SendGridOptions"));
            services.Configure<SmtpOptions>(Configuration.GetSection("SmtpOptions"));
            services.Configure<SuperAdminDefaultOptions>(Configuration.GetSection("SuperAdminDefaultOptions"));
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<INumberSequence, Services.NumberSequence>();
            services.AddTransient<IRoles, Roles>();
            services.AddTransient<ITasks, Tasks>();
            services.AddTransient<IFunctional, Functional>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddAuthentication();
        }

        public void Configure(IApplicationBuilder app, Microsoft.Extensions.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                //app.UseExceptionHandler("/Home/Error");
                app.UseStaticFiles();
                app.UseCookiePolicy();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseCookiePolicy();
            }
           
            app.UseResponseCompression();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseCookiePolicy();
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=GeraFinUser}/{action=Index}");
            });
        }
    }
}