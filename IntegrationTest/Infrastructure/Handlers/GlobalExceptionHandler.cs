using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace IntegrationTest.Infrastructure
{
    public class GlobalExceptionHandler : ExceptionHandler
    {
        public override void Handle(ExceptionHandlerContext context)
        {
            context.Result = new ErrorResult
            {
                Request = context.ExceptionContext.Request,
                Message = "Oops! Sorry! Something went wrong.",
                MessageDetail = context.ExceptionContext.Exception.Message
            };
            base.Handle(context);
        }

        private class ErrorResult : IHttpActionResult
        {
            public HttpRequestMessage Request { get; set; }

            public string Message { get; set; }

            public string MessageDetail { get; set; }

            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.InternalServerError, new ResponsePackage(Message, MessageDetail));
                return Task.FromResult(response);
            }
        }
    }
}