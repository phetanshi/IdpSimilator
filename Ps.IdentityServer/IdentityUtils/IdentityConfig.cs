using Duende.IdentityServer.Models;
using IdentityModel;

namespace Ps.IdentityServer.IdentityUtils;

public static class IdentityConfig
{
    public static IEnumerable<Client> Clients => new Client[] 
    {
        new Client
        {
            ClientId = "swaggerapp",
            ClientSecrets = { new Secret("padmasekhar".Sha256()) },
            AllowedGrantTypes = GrantTypes.Implicit,
            RedirectUris = { "https://localhost:7133/swagger/oauth2-redirect.html", "https://oauth.pstmn.io/v1/callback" },
            PostLogoutRedirectUris = { "https://localhost:7133/signout-callback-oidc" },

            AllowOfflineAccess = true,
            AllowedScopes = {"openid", "profile", "api.read", "api.write"},
            AlwaysIncludeUserClaimsInIdToken = true,
            AlwaysSendClientClaims = true,
            AllowAccessTokensViaBrowser = true
        },
        new Client
        {
            ClientId = "reactapp",
            ClientSecrets = { new Secret("padmasekhar".Sha256()) },
            AllowedGrantTypes = GrantTypes.Code,
            RedirectUris = { "https://localhost:3000/redirect", "http://localhost:3000/redirect", "https://oauth.pstmn.io/v1/callback" },
            FrontChannelLogoutUri = "http://localhost:3000",
            PostLogoutRedirectUris = { "https://localhost:3000/signout-callback-oidc", "http://localhost:3000/signout-callback-oidc" },

            AllowOfflineAccess = true,
            AllowedScopes = {"openid", "profile", "api.read", "api.write"},
            AlwaysIncludeUserClaimsInIdToken = true,
            AlwaysSendClientClaims = true,
            AllowedCorsOrigins = { "https://localhost:3000", "http://localhost:3000", "https://localhost:8091", "http://localhost:8092" },
            AllowAccessTokensViaBrowser = true
        }
    };

    public static IEnumerable<IdentityResource> IdentityResources => new IdentityResource[]
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
        new IdentityResources.Email(),
        new IdentityResource { Name = "role", UserClaims = new List<string> { "role" } },
        new IdentityResource { Name = "name", UserClaims = new List<string> { JwtClaimTypes.Name } }
    };

    public static IEnumerable<ApiScope> ApiScopes => new ApiScope[]
    {
        new ApiScope("weatherapi.read"),
        new ApiScope("weatherapi.write"),
        new ApiScope("api.read"),
        new ApiScope("api.write")
    };

    public static IEnumerable<ApiResource> ApiResources => new ApiResource[]
    {
        new ApiResource("weatherapi")
        {
            Scopes = new List<string> { "weatherapi.read", "weatherapi.write", "openid", "profile" },
            ApiSecrets = new List<Secret> { new Secret(ServerConstants.DEFAULT_SEC.Sha256()) },
            UserClaims = new List<string> { JwtClaimTypes.Name, JwtClaimTypes.Email, JwtClaimTypes.Role, JwtClaimTypes.Subject }
        },
        new ApiResource("restapi")
        {
            Scopes = new List<string> { "weatherapi.read", "weatherapi.write", "api.read", "api.write", "openid", "profile" },
            ApiSecrets = new List<Secret> { new Secret(ServerConstants.DEFAULT_SEC.Sha256()) },
            UserClaims = new List<string> { JwtClaimTypes.Name, JwtClaimTypes.Email, JwtClaimTypes.Role, JwtClaimTypes.Subject }
        }
    };
}
