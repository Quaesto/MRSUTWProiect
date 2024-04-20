using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using MRSTWEb.BusinessLogic.Interfaces;
using MRSTWEb.BusinessLogic.Services;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

[assembly: OwinStartup(typeof(MRSTWEb.App_Start.Startup))]
namespace MRSTWEb.App_Start
{
    public class Startup
    {
        IServiceCreator serviceCreator = new ServiceCreator();

        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext(CreateUserService);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
            });

        }
        private IUserService CreateUserService()
        {
            return serviceCreator.CreateUserService();
        }
    }
}