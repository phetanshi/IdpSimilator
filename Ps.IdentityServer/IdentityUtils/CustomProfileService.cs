using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using Ps.IdentityServer.Data;
using System.Security.Claims;

namespace Ps.IdentityServer.IdentityUtils;

public class CustomProfileService(UserManager<ApplicationUser> _userManager) : IProfileService
{
    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);
        var userClaims = await _userManager.GetClaimsAsync(user);

        var claims = new List<Claim>
        {
            new Claim(IdentityModel.JwtClaimTypes.Email, user.Email),
            new Claim(IdentityModel.JwtClaimTypes.Name, user.UserName),
            new Claim("group", "custom claim")
        };

        if(userClaims is not null)
        {
            claims.AddRange(userClaims);
        }
        context.IssuedClaims.AddRange(claims);
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);
        context.IsActive = user != null;
    }
}
