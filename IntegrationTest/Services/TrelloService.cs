using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IntegrationTest.Infrastructure;
using IntegrationTest.Models;
using LazyCache;
using RestSharp;

namespace IntegrationTest.Services
{
    public interface ITrelloService
    {
        string ApiKey { get; }
        string Token { get; }
        string GetAuthorizationUrl(string apiKey = null);
        Task<Token> GetTokenAsync();
        Task<Member> GetMemberAsync(string memberId = null);
        Task<List<Board>> GetBoardsAsync(string memberId = null);
        Task<Board> GetBoardAsync(string boardId);
        Task<List<Card>> GetCardsAsync(string boardId);
        Task<Card> GetCardAsync(string cardId);
        Task<List<CommentCardAction>> GetCommentCardActionsAsync(string cardId);
        Task<CommentCardAction> GetCommentCardActionAsync(string actionId);
        Task<CommentCardAction> PostCommentCardActionAsync(string cardId, string comment);
    }

    public class TrelloService : ITrelloService
    {
        private readonly IAppCache _cache;
        private readonly ITrelloRestClient _restClient;
        private readonly AppDBContext _dbContext;

        public string ApiKey
        {
            get
            {
                return _cache.GetOrAdd("apiKey",
                    () => _dbContext.AppKeys.Select(a => a.ApiKey).SingleOrDefault(),
                    DateTimeOffset.Now.AddMinutes(5));
            }
        }

        public string Token
        {
            get
            {
                return _cache.GetOrAdd("token",
                    () => _dbContext.KeyTokens.Where(kt => kt.ApiKey == ApiKey).Select(kt => kt.Token)
                        .SingleOrDefault(),
                    DateTimeOffset.Now.AddMinutes(5));
            }
        }

        public string MemberId
        {
            get
            {
                // TODO: Improvement - Save the memberId in the local db so that we don't need to retrieve it remotely
                return _cache.GetOrAdd("member",
                    () => GetTokenAsync().Result.IdMember,
                    DateTimeOffset.Now.AddMinutes(5));
            }
        }

        public TrelloService(AppDBContext dbContext, ITrelloRestClient restClient, IAppCache cache)
        {
            _dbContext = dbContext;
            _restClient = restClient;
            _cache = cache;
        }

        public string GetAuthorizationUrl(string apiKey = null)
        {
            // We are asking for a perpetual token for demonstration. We could ask for tokens with expiry date, but for now, we want to focus on other aspects of the demo
            return $"{_restClient.BaseUrl}authorize?key={apiKey ?? ApiKey}&name=Integration+Test&expiration=never&response_type=token&scope=read,write";
        }

        public Task<Token> GetTokenAsync()
        {
            var request = BuildRestRequest("tokens/{token}?");
            return _restClient.ExecuteAsync<Token>(request);
        }

        public Task<Member> GetMemberAsync(string memberId = null)
        {
            var request = BuildRestRequest("members/{memberId}?", new { memberId = memberId ?? MemberId });
            return _restClient.ExecuteAsync<Member>(request);
        }

        public Task<List<Board>> GetBoardsAsync(string memberId = null)
        {
            var request = BuildRestRequest("members/{memberId}/boards?filter=all&", new { memberId = memberId ?? MemberId });
            return _restClient.ExecuteAsync<List<Board>>(request);
        }

        public Task<Board> GetBoardAsync(string boardId)
        {
            var request = BuildRestRequest("boards/{boardId}?", new { boardId });
            return _restClient.ExecuteAsync<Board>(request);
        }

        public Task<List<Card>> GetCardsAsync(string boardId)
        {
            var request = BuildRestRequest("boards/{boardId}/cards?", new { boardId });
            return _restClient.ExecuteAsync<List<Card>>(request);
        }

        public Task<Card> GetCardAsync(string cardId)
        {
            var request = BuildRestRequest("cards/{cardId}?", new { cardId });
            return _restClient.ExecuteAsync<Card>(request);
        }

        public Task<List<CommentCardAction>> GetCommentCardActionsAsync(string cardId = null)
        {
            var request = BuildRestRequest("cards/{cardId}/actions?filter=commentCard&", new { cardId });
            return _restClient.ExecuteAsync<List<CommentCardAction>>(request);
        }

        public Task<CommentCardAction> GetCommentCardActionAsync(string actionId)
        {
            var request = BuildRestRequest("actions/{actionId}?", new { actionId });
            return _restClient.ExecuteAsync<CommentCardAction>(request);
        }

        public Task<CommentCardAction> PostCommentCardActionAsync(string cardId, string comment)
        {
            var request = BuildRestRequest("cards/{cardId}/actions/comments?text={comment}&", new { cardId, comment }, Method.POST);
            return _restClient.ExecuteAsync<CommentCardAction>(request);
        }

        private RestRequest BuildRestRequest(string resource, object parameters = null, Method method = Method.GET)
        {
            var request = new RestRequest {Resource = $"{resource}token={{token}}&key={{key}}"};
            var propertyInfos = parameters?.GetType().GetProperties();
            if (propertyInfos != null)
            {
                foreach (PropertyInfo prop in propertyInfos)
                {
                    request.AddParameter(prop.Name, prop.GetValue(parameters, null).ToString(),
                        ParameterType.UrlSegment);
                }
            }
            request.AddParameter("token", Token, ParameterType.UrlSegment);
            request.AddParameter("key", ApiKey, ParameterType.UrlSegment);
            request.Method = method;
            return request;
        }
    }
}