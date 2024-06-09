using Hangfire;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using MRSTWEb.BusinessLogic.Interfaces;
using MRSTWEb.BusinessLogic.Services;
using Owin;
using System;
using Microsoft.Owin.Security.Google;
using MRSTWEb.Domain.Identity;

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
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "800643618878-kp8dt4fo0sc7h0nmt05i4in8rdni36g9.apps.googleusercontent.com",
                ClientSecret = "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"
            });
            GlobalConfiguration.Configuration.UseSqlServerStorage("DefaultConnection");
            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}
