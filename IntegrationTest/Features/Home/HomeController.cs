using System;
using System.Web.Http;
using System.Web.Mvc;
using IntegrationTest.Services;

namespace IntegrationTest.Features
{
    public class HomeController : Controller
    {
        private readonly ITrelloService _trelloService;


        public HomeController(ITrelloService trelloService)
        {
            _trelloService = trelloService ?? throw new ArgumentNullException(nameof(trelloService));
        }

        public ActionResult Index()
        {
            return View();
        }

        [System.Web.Http.HttpPost]
        public ActionResult Authorize([FromBody]string apiKey = null)
        {
            return Redirect(_trelloService.GetAuthorizationUrl(!string.IsNullOrEmpty(apiKey) ? apiKey : null));
        }
    }
}