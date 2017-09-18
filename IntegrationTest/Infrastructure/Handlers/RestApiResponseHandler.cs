using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;

namespace IntegrationTest.Infrastructure
{
    public class RestApiResponseHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return BuildApiResponseError(request, response);
            }

            return response;
        }

        private HttpResponseMessage BuildApiResponseError(HttpRequestMessage request, HttpResponseMessage response)
        {
            object content;
            string message = null;
            string messageDetail = null;
            List<string> errors = null;

            if (response.TryGetContentValue(out content))
            {
                var error = content as HttpError;
                if (error != null)
                {
                    // Insert the ModelState errors
                    if (error.ModelState != null)
                    {
                        message = "There are validation errors in your request";
                        // Read as string
                        var httpErrorObject = response.Content.ReadAsStringAsync().Result;

                        // Convert to anonymous object
                        var anonymousErrorObject =
                            new {message = "", ModelState = new Dictionary<string, string[]>()};

                        // Deserialize anonymous object
                        var deserializedErrorObject = JsonConvert.DeserializeAnonymousType(httpErrorObject,
                            anonymousErrorObject);

                        // Get error messages from ModelState object
                        var modelStateValues =
                            deserializedErrorObject.ModelState.Select(kvp => string.Join("\n", kvp.Value))
                                .ToList();

                        errors = new List<string>();
                        for (var i = 0; i < modelStateValues.Count; i++)
                        {
                            errors.Add(modelStateValues.ElementAt(i));
                        }
                    }
                    else
                    {
                        message = error.Message;
                        messageDetail = error.MessageDetail;
                    }
                } else if (content is ResponsePackage)
                {
                    var package = (ResponsePackage) content;
                    message = (string)package.Message;
                    messageDetail = (string)package.MessageDetail;
                }
            }
            else
            {
                message = response.ReasonPhrase;
                //messageDetails = response.
            }

            // Create a new response
            var newResponse = request.CreateResponse(response.StatusCode, new ResponsePackage(message, messageDetail, errors));

            // Add Back the Response Headers
            foreach (var header in response.Headers)
            {
                newResponse.Headers.Add(header.Key, header.Value);
            }

            return newResponse;
        }
    }
}