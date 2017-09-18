using System.Web.Mvc;
using IntegrationTest.Infrastructure;
using Newtonsoft.Json;

namespace IntegrationTest.Features
{
    public class ErrorController : Controller
    {
        public ActionResult CustomError()
        {
            var exception = ((HandleErrorInfo) ViewData.Model).Exception;
            var content = JsonConvert.SerializeObject(new ResponsePackage(exception.Message));
            return Content( content, "application/json" );
        }
    }
}