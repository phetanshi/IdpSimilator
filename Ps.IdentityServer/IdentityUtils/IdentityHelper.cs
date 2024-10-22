using IdentityModel;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Security.Cryptography;

namespace Ps.IdentityServer.IdentityUtils;

public static class IdentityHelper
{
    public static  RsaSecurityKey GetRSAPrivateKey(this IConfiguration configuration)
    {
        var rsapri = RSA.Create();
        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        rsapri.FromXmlString(File.ReadAllText($"{path}\\keys\\privatefile.xml"));

        return new RsaSecurityKey(rsapri)
        {
            KeyId = CryptoRandom.CreateUniqueId(16, CryptoRandom.OutputFormat.Hex)
        };
    }
}
