using Microsoft.AspNetCore.Identity;

namespace Ps.IdentityServer.Data;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string Surname { get; set; }
}
