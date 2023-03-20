using Microsoft.Extensions.Logging;
using Umbraco.AuthorizedServices.Configuration;
using Umbraco.AuthorizedServices.Models;
using Umbraco.Cms.Core.Cache;
using Umbraco.Extensions;

namespace Umbraco.AuthorizedServices.Services.Implement;

internal abstract class AuthorizedServiceBase
{
    private readonly AuthorizedServiceSettings _authorizedServiceSettings;
    private readonly ITokenFactory _tokenFactory;

    public AuthorizedServiceBase(
        IHttpClientFactory httpClientFactory,
        AppCaches appCaches,
        ITokenFactory tokenFactory,
        ITokenStorage tokenStorage,
        IAuthorizationRequestSender authorizationRequestSender,
        ILogger logger,
        AuthorizedServiceSettings authorizedServiceSettings)
    {
        HttpClientFactory = httpClientFactory;
        AppCaches = appCaches;
        _tokenFactory = tokenFactory;
        TokenStorage = tokenStorage;
        AuthorizationRequestSender = authorizationRequestSender;
        Logger = logger;
        _authorizedServiceSettings = authorizedServiceSettings;
    }

    protected IHttpClientFactory HttpClientFactory { get; }

    protected AppCaches AppCaches { get; }

    protected ITokenStorage TokenStorage { get; }

    protected IAuthorizationRequestSender AuthorizationRequestSender { get; }

    protected ILogger Logger { get; }

    protected ServiceDetail GetServiceDetail(string serviceAlias)
    {
        ServiceDetail? serviceDetail = _authorizedServiceSettings.Services.SingleOrDefault(x => x.Alias == serviceAlias);
        if (serviceDetail == null)
        {
            throw new InvalidOperationException($"Cannot find service config for service alias '{serviceAlias}'");
        }

        return serviceDetail;
    }

    protected async Task<Token> CreateTokenFromResponse(string serviceAlias, ServiceDetail serviceDetail, HttpResponseMessage response)
    {
        var responseContent = await response.Content.ReadAsStringAsync();
        return _tokenFactory.CreateFromResponseContent(responseContent, serviceAlias, serviceDetail);
    }

    protected void StoreToken(string serviceAlias, Token token)
    {
        // Add the access token details to the cache.
        var cacheKey = GetTokenCacheKey(serviceAlias);
        AppCaches.RuntimeCache.InsertCacheItem(cacheKey, () => token);

        // Save the refresh token into the storage.
        TokenStorage.SaveToken(serviceAlias, token);
    }

    private static string GetTokenCacheKey(string serviceAlias) => $"Umbraco_AuthorizedServiceToken_{serviceAlias}";
}
