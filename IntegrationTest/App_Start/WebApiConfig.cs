using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using IntegrationTest.Infrastructure;

namespace IntegrationTest
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.Services.Replace(typeof(IExceptionHandler), new GlobalExceptionHandler());
            config.MessageHandlers.Add(new RestApiResponseHandler());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{*uri}",
                new {controller = "Default", uri = RouteParameter.Optional});
        }
    }
}
