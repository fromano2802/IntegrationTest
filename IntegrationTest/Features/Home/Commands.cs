using System;
using System.Data.Entity;
using System.Threading.Tasks;
using IntegrationTest.Infrastructure;
using IntegrationTest.Models;
using LazyCache;
using MediatR;

namespace IntegrationTest.Features
{
    public class SaveApiKey : IRequest<AppKey>
    {
        public string ApiKey { get; }

        public SaveApiKey(string apiKey)
        {
            ApiKey = apiKey;
        }
    }

    public class RemoveApiKey : IRequest<bool>
    {
        public string ApiKey { get; }

        public RemoveApiKey(string apiKey)
        {
            ApiKey = apiKey;
        }
    }

    public class SaveToken : IRequest<KeyToken>
    {
        public string ApiKey { get; }
        public string Token { get; }

        public SaveToken(string apiKey, string token)
        {
            ApiKey = apiKey;
            Token = token;
        }
    }

    public class RemoveToken : IRequest<bool>
    {
        public string ApiKey { get; }

        public RemoveToken(string apiKey)
        {
            ApiKey = apiKey;
        }
    }

    public class SaveApiKeyHandler : IAsyncRequestHandler<SaveApiKey, AppKey>
    {
        private readonly AppDBContext _dbContext;
        private readonly ILogger  _logger;

        public SaveApiKeyHandler(AppDBContext dbContext, ILogger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<AppKey> Handle(SaveApiKey message)
        {
            var appKey = await _dbContext.AppKeys
                .FirstOrDefaultAsync(a => a.ApiKey == message.ApiKey);

            if (appKey == null)
            {
                appKey = _dbContext.AppKeys.Add(new AppKey { ApiKey = message.ApiKey });

                await _dbContext.SaveChangesAsync();
                _logger.Info("ApiKey saved.", null);
            }

            return appKey;
        }
    }

    public class RemoveApiKeyHandler : IAsyncRequestHandler<RemoveApiKey, bool>
    {
        private readonly AppDBContext _dbContext;
        private readonly ILogger  _logger;
        private readonly IAppCache _cache;

        public RemoveApiKeyHandler(AppDBContext dbContext, ILogger logger, IAppCache cache)
        {
            _dbContext = dbContext;
            _logger = logger;
            _cache = cache;
        }

        public async Task<bool> Handle(RemoveApiKey message)
        {
            var appKey = await _dbContext.AppKeys
                .FirstOrDefaultAsync(kt => kt.ApiKey == message.ApiKey);
            if (appKey == null)
                return false;

            using (var transaction = _dbContext.Database.BeginTransaction()) 
            { 
                try 
                { 
                    var keyToken = await _dbContext.KeyTokens
                        .FirstOrDefaultAsync(kt => kt.ApiKey == message.ApiKey);
                    if (keyToken == null)
                        return false;
                    _dbContext.KeyTokens.Remove(keyToken);
 
                    _dbContext.AppKeys.Remove(appKey);
                    await _dbContext.SaveChangesAsync();

                    _logger.Info("ApiKey removed.", null);
                } 
                catch (Exception ex) 
                { 
                    transaction.Rollback();
                    _logger.Error("failed to remove ApiKey.", ex);
                    throw;
                } 
            }

            _cache.ClearCache();

            return true;
        }
    }

    public class SaveTokenHandler : IAsyncRequestHandler<SaveToken, KeyToken>
    {
        private readonly AppDBContext _dbContext;
        private readonly ILogger  _logger;

        public SaveTokenHandler(AppDBContext dbContext, ILogger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<KeyToken> Handle(SaveToken message)
        {
            var keyToken = await _dbContext.KeyTokens
                .FirstOrDefaultAsync(kt => kt.ApiKey == message.ApiKey);

            if (keyToken == null)
            {
                keyToken = _dbContext.KeyTokens.Add(new KeyToken { ApiKey = message.ApiKey, Token = message.Token });

                await _dbContext.SaveChangesAsync();
                _logger.Info("Token saved.", null);
            }

            return keyToken;
        }
    }

    public class RemoveTokenHandler : IAsyncRequestHandler<RemoveToken, bool>
    {
        private readonly AppDBContext _dbContext;
        private readonly ILogger  _logger;
        private readonly IAppCache _cache;

        public RemoveTokenHandler(AppDBContext dbContext, ILogger logger, IAppCache cache)
        {
            _dbContext = dbContext;
            _logger = logger;
            _cache = cache;
        }

        public async Task<bool> Handle(RemoveToken message)
        {
            var keyToken = await _dbContext.KeyTokens
                .FirstOrDefaultAsync(kt => kt.ApiKey == message.ApiKey);
            if (keyToken == null)
                return false;

            _dbContext.KeyTokens.Remove(keyToken);
            await _dbContext.SaveChangesAsync();
            _logger.Info("Token removed.", null);

            _cache.ClearCache();

            return true;
        }
    }
}