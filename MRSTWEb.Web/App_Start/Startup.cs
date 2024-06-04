using Hangfire;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using MRSTWEb.BusinessLogic.Interfaces;
using MRSTWEb.BusinessLogic.Services;
using Owin;
using System;

[assembly: OwinStartup(typeof(MRSTWEb.App_Start.Startup))]
namespace MRSTWEb.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var dataProtectionProvider = app.GetDataProtectionProvider();
            IServiceCreator serviceCreator = new ServiceCreator(dataProtectionProvider);

            app.CreatePerOwinContext(() => serviceCreator.CreateUserService());
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                ExpireTimeSpan = TimeSpan.FromMinutes(10),
                CookieName = "ApplicationCookie",
                SlidingExpiration = true,

            });

            GlobalConfiguration.Configuration.UseSqlServerStorage("DefaultConnection");
            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}
