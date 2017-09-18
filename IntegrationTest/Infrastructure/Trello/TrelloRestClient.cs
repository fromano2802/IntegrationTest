using System;
using System.Threading.Tasks;
using RestSharp;

namespace IntegrationTest.Infrastructure
{
    public interface ITrelloRestClient : IRestClient
    {
        Task<TEntity> ExecuteAsync<TEntity>(RestRequest request) where TEntity : new();
    }

    public class TrelloRestClient : RestClient, ITrelloRestClient
    {
        public TrelloRestClient(Uri baseUrl) : base(baseUrl)
        {
            
        }

        public Task<TEntity> ExecuteAsync<TEntity>(RestRequest request) where TEntity : new()
        {
            var tcs = new TaskCompletionSource<TEntity>();
            ExecuteAsync<TEntity>(request, (response, handle) =>
            {
                if (response.ErrorException != null)
                {
                    // TODO: Improvement - throw different exceptions depending on the response status
                    var exception = SetTrelloException(response);
                    tcs.SetException(exception);
                }
                else
                {
                    tcs.SetResult(response.Data);
                }
            });
            return tcs.Task;
        }

        private ApplicationException SetTrelloException<TEntity>(IRestResponse<TEntity> response)
        {
            var message = $"{typeof(TEntity).Name} request error: {response.Content}. Check logs for more info.";
            var exception = new ApplicationException(message, response.ErrorException);
            return exception;
        }
    }
}