using System.ComponentModel.Composition;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using IntegrationTest.Services;


namespace IntegrationTest.Infrastructure
{
    public class WebApiAuthorizationAttribute : BaseAuthorizationAttribute
    {
        [Import]
        public ITrelloService TrelloService { get; set; }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var apiKey = TrelloService.ApiKey;

            if (apiKey != null)
            {
                var token = TrelloService.Token;
                if (token == null)
                {
                    var response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized,
                        new ResponsePackage(HttpStatusCode.Unauthorized.ToString(),
                            "Cannot find a Token associated to the given ApiKey."));
                    throw new HttpResponseException(response);
                }
            }
            else
            {
                var response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized,
                    new ResponsePackage(HttpStatusCode.Unauthorized.ToString(),
                        "Cannot find ApiKey, please go to https://trello.com/app-key to generate one."));
                throw new HttpResponseException(response);
            }
        }
    }
}