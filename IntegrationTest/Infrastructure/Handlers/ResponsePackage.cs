using System.Collections.Generic;
using Newtonsoft.Json;

namespace IntegrationTest.Infrastructure
{
    public class ResponsePackage
    {
        public ResponsePackage(string message = null, string messageDetail = null, List<string> errors = null)
        {
            Message = message;
            MessageDetail = messageDetail;
            Errors = errors;
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Message { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object MessageDetail { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Errors { get; set; }
    }
}