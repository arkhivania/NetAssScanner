using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Nailhang.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        class NinjectControllerFactory : DefaultControllerFactory
        {
            private readonly IKernel kernel;

            public NinjectControllerFactory(IKernel kernel)
            {
                this.kernel = kernel;
            }

            protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
            {
                if (controllerType != null)
                    return (IController)kernel.Get(controllerType);

                return base.GetControllerInstance(requestContext, controllerType);
            }
            
        }

        protected void Application_Start()
        {
            var kernel = new StandardKernel(new Nailhang.Mongodb.Module());
            ControllerBuilder.Current.SetControllerFactory(new NinjectControllerFactory(kernel));

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
