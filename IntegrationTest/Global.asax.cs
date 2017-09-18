using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Security.Authentication;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using IntegrationTest.Features;
using IntegrationTest.Infrastructure;
using MediatR.SimpleInjector;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;
using SimpleInjector.Integration.WebApi;
using IFilterProvider = System.Web.Http.Filters.IFilterProvider;

namespace IntegrationTest
{
    public class Global : HttpApplication
    {
        protected void Application_Start()
        {
            MvcHandler.DisableMvcResponseHeader = true;

            var config = GlobalConfiguration.Configuration;

            // Initialize Json Formatter
            var jsonFormatter = JsonHelpers.InitJsonFormatter();
            config.Services.Replace(typeof(IContentNegotiator), new JsonContentNegotiator(jsonFormatter));

            // Initialize SimpleInjector
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();
            GlobalConfiguration.Configuration.Services.Remove(typeof(IFilterProvider),
                GlobalConfiguration.Configuration.Services.GetFilterProviders()
                    .OfType<ActionDescriptorFilterProvider>().Single());
            GlobalConfiguration.Configuration.Services.Add(
                typeof(IFilterProvider),
                new SimpleInjectorFilterProvider(container));

            // Register dependencies
            container.RegisterApplicationDependencies();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            container.RegisterWebApiControllers(config);

            container.BuildMediator(GetAssemblies());

            container.Verify(VerificationOption.VerifyAndDiagnose);
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver =
                new SimpleInjectorWebApiDependencyResolver(container);

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new FeatureViewLocationRazorViewEngine());
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var logger = DependencyResolver.Current.GetService<NLogProxy<Global>>();

            try
            {
                var lastError = Server.GetLastError();
                logger.Error(lastError);

                var httpContext = ((HttpApplication) sender).Context;

                var controller = new ErrorController();
                var routeData = new RouteData();
                routeData.Values["controller"] = "Error";
                routeData.Values["action"] = "CustomError";
                var statusCode = 500;

                if (lastError is HttpException)
                {
                    var httpEx = lastError as HttpException;
                    statusCode = httpEx.GetHttpCode();
                }
                else if (lastError is AuthenticationException)
                {
                    statusCode = 403;
                }

                httpContext.ClearError();

                httpContext.Response.Clear();
                httpContext.Response.StatusCode = statusCode;
                httpContext.Response.TrySkipIisCustomErrors = true;

                controller.ViewData.Model = new HandleErrorInfo(lastError, " ", " ");
                ((IController) controller).Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            yield return Assembly.GetExecutingAssembly();
        }
    }
}
