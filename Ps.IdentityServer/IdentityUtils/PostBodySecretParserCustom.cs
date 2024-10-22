using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;
using IdentityModel;

namespace Ps.IdentityServer.IdentityUtils;

public class PostBodySecretParserCustom(ILogger<PostBodySecretParserCustom> _logger) : ISecretParser
{
    public string AuthenticationMethod => OidcConstants.EndpointAuthenticationMethods.PostBody;

    public async Task<ParsedSecret?> ParseAsync(HttpContext context)
    {
        _logger.LogDebug("Custom start parsing for secret in post body");

        var body = await context.Request.ReadFormAsync();
        if (body != null)
        {
            var id = body["client_id"].FirstOrDefault();
            var secret = body["client_secret"].FirstOrDefault();

            return new ParsedSecret
            {
                Id = id,
                Credential = secret ?? "padmasekhar",
                Type = IdentityServerConstants.ParsedSecretTypes.SharedSecret
            };
        }
        return null;
    }
}
