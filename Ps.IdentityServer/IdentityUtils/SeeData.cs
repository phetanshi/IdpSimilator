using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Models;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ps.IdentityServer.Data;
using Serilog;
using System.Security.Claims;

namespace Ps.IdentityServer.IdentityUtils;

public static class SeeData
{
    public static void EnsureSeedData(this WebApplication app)
    {
        using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

            var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            context.Database.Migrate();
            context.SeedClients();
            context.SeedIdentityResources();
            context.SeedApiScope();
            context.SeedApiResources();
            //context.SeedIdentityProviders();
            scope.SeedUsers();

        }
    }

    private static void SeedClients(this ConfigurationDbContext context)
    {
        try
        {
            if(!context.Clients.Any())
            {
                Serilog.Log.Debug("Clients are being populated");
                foreach(var client in IdentityConfig.Clients.ToList())
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }
            else
            {
                Serilog.Log.Debug("Clients are already populated");
            }
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, $"Error occurred in SeeData.SeedClients");
            throw;
        }
    }

    private static void SeedIdentityResources(this ConfigurationDbContext context) 
    {
        try
        {
            if (!context.IdentityResources.Any())
            {
                Serilog.Log.Debug("IdentityResources are being populated");
                foreach (var resources in IdentityConfig.IdentityResources.ToList())
                {
                    context.IdentityResources.Add(resources.ToEntity());
                }
                context.SaveChanges();
            }
            else
            {
                Serilog.Log.Debug("IdentityResources are already populated");
            }
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, $"Error occurred in SeeData.SeedIdentityResources");
            throw;
        }
    }

    private static void SeedApiScope(this ConfigurationDbContext context)
    {
        try
        {
            if (!context.ApiScopes.Any())
            {
                Serilog.Log.Debug("ApiScopes are being populated");
                foreach (var apiScp in IdentityConfig.ApiScopes.ToList())
                {
                    context.ApiScopes.Add(apiScp.ToEntity());
                }
                context.SaveChanges();
            }
            else
            {
                Serilog.Log.Debug("ApiScopes are already populated");
            }
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, $"Error occurred in SeeData.SeedApiScope");
            throw;
        }
    }

    private static void SeedApiResources(this ConfigurationDbContext context)
    {
        try
        {
            if (!context.ApiResources.Any())
            {
                Serilog.Log.Debug("ApiResources are being populated");
                foreach (var apiRes in IdentityConfig.ApiResources.ToList())
                {
                    context.ApiResources.Add(apiRes.ToEntity());
                }
                context.SaveChanges();
            }
            else
            {
                Serilog.Log.Debug("ApiResources are already populated");
            }
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, $"Error occurred in SeeData.SeedApiResources");
            throw;
        }
    }

    private static void SeedIdentityProviders(this ConfigurationDbContext context)
    {
        try
        {
            if(!context.IdentityProviders.Any())
            {
                Log.Debug("OIDC identity providers being populated");
                context.IdentityProviders.Add(new OidcProvider
                {
                    Scheme = "demoidsrv",
                    DisplayName = "IdentityServer",
                    Authority = "https://demo.duendesoftware.com",
                    ClientId = "login"
                }.ToEntity());
                
                context.SaveChanges();
            }
            else
            {
                Log.Debug("OIDC IdentityProviders already populated");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred in SeeData.SeedIdentityProviders");
            throw;
        }
    }

    private static void SeedUsers(this IServiceScope scope)
    {
        scope.CreateUser("e680527", "padmasekhar.ps16@gmail.com", "Padmasekhar", "Pottepalem");
        scope.CreateUser("narmada", "narmada.ps16@gmail.com", "Narmada", "Chittela");
    }

    private static void CreateUser(this IServiceScope scope, string username, string email, string firstName, string familyName, string webSite = "https://padmasekhar.com", string password = "Mypwd123$")
    {
        try
        {
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var user = userMgr.FindByNameAsync(username).Result;
            if (user == null)
            {
                user = new ApplicationUser
                {
                    FirstName = firstName,
                    Surname = familyName,
                    Email = email,
                    UserName = username,
                    EmailConfirmed = true,
                };
                var result = userMgr.CreateAsync(user, password).Result;

                if (!result.Succeeded)
                    throw new Exception(result.Errors.First().Description);

                result = userMgr.AddClaimsAsync(user, new Claim[]
                {
                    new Claim(JwtClaimTypes.GivenName, firstName),
                    new Claim(JwtClaimTypes.FamilyName, familyName),
                    new Claim(JwtClaimTypes.WebSite, webSite)
                }).Result;

                if(!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                Log.Debug($"{firstName} created");
            }
            else
            {
                Log.Debug($"{firstName} already exists");
            }
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, $"Error occurred in SeeData.CreateUser - username : {username}; firstName : {firstName}");
            throw;
        }
    }
}
