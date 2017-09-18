using System;
using RestSharp;

namespace IntegrationTest.Infrastructure
{
    public sealed class TrelloRestClientFactory
    {
        public static TrelloRestClient Create()
        {
            var baseUrl = new Uri("https://trello.com/1/");
            return new TrelloRestClient(baseUrl);
        }
    }
}