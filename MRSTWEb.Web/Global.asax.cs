using MRSTWEb.BusinessLogic.Infrastructure;
using MRSTWEb.Util;
using Ninject.Modules;
using Ninject.Web.Mvc;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace MRSTWEb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            NinjectModule serviceModule = new ServiceModule("Connection");
            NinjectModule cartModule = new CartModule();


            var kernel = new StandardKernel(serviceModule, cartModule);
            DependencyResolver.SetResolver(new NinjectDependencyResolver(kernel));
        }
    }
}
