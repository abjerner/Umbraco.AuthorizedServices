namespace Umbraco.AuthorizedServices.Configuration;

public enum TokenRequestContentFormat
{
    Querystring,
    FormUrlEncoded
}

public class AuthorizedServiceSettings
{
    public string TokenEncryptionKey { get; set; } = string.Empty;

    public List<ServiceDetail> Services { get; set; } = new List<ServiceDetail>();
}

public class ServiceDetail
{
    public string Alias { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string Icon { get; set; } = "icon-command";

    public string ApiHost { get; set; } = string.Empty;

    public string IdentityHost { get; set; } = string.Empty;

    public string TokenHost { get; set; } = string.Empty;

    public string RequestIdentityPath { get; set; } = string.Empty;

    public bool AuthorizationRequestsRequireRedirectUri { get; set; } = false;

    public string RequestTokenPath { get; set; } = string.Empty;

    public TokenRequestContentFormat RequestTokenFormat { get; set; } = TokenRequestContentFormat.Querystring;

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public string Scopes { get; set; } = string.Empty;

    public string AccessTokenResponseKey { get; set; } = "access_token";

    public string RefreshTokenResponseKey { get; set; } = "refresh_token";

    public string ExpiresInResponseKey { get; set; } = "expires_in";

    public string? SampleRequest { get; set; }

    internal string GetTokenHost() => string.IsNullOrWhiteSpace(TokenHost)
        ? IdentityHost
        : TokenHost;
}
