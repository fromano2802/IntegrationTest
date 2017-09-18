using System.Net.Http.Formatting;
using Newtonsoft.Json.Serialization;

namespace IntegrationTest.Infrastructure
{
    public static class JsonHelpers
    {
        public static JsonMediaTypeFormatter InitJsonFormatter()
        {
            var formatter = new JsonMediaTypeFormatter
            {
                SerializerSettings =
                {
#if DEBUG
                    Formatting = Newtonsoft.Json.Formatting.Indented,
#endif
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }
            };

            formatter.SerializerSettings.Converters?.Add(
                new Newtonsoft.Json.Converters.StringEnumConverter());

            return formatter;
        }
    }
}