using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Ps.IdentityServer.Data;
using Ps.IdentityServer.IdentityUtils;
using Serilog;
using System.Reflection;


Log.Logger = new LoggerConfiguration().WriteTo
    .Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
    .MinimumLevel.Information()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var conn = builder.Configuration.GetConnectionString("AppIdentityDb");
    var assemblyName = typeof(ApplicationDbContext).Assembly.GetName().Name;
    options.UseSqlServer(conn, sqlOptions => sqlOptions.MigrationsAssembly(assemblyName));
});




#region Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddTransient<ISecretParser, PostBodySecretParserCustom>();

var identityDbConn = builder.Configuration.GetConnectionString("AppIdentityDb");

var identityAssemblyName = typeof(IdentityConfig).GetTypeInfo().Assembly.GetName().Name;
var key = builder.Configuration.GetRSAPrivateKey();



builder.Services.AddIdentityServer(options =>
{
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;
    options.EmitStaticAudienceClaim = true;
    options.KeyManagement.Enabled = false; // Please uncomments if you want to use your own key.
})
    .AddSecretParser<PostBodySecretParserCustom>()
    .AddSigningCredential(key, SecurityAlgorithms.RsaSha256) //Please uncomments if you have private key
    .AddConfigurationStore(options => options.ConfigureDbContext = x => x.UseSqlServer(identityDbConn, sqlOptions => sqlOptions.MigrationsAssembly(identityAssemblyName)))
    .AddOperationalStore(options => options.ConfigureDbContext = x => x.UseSqlServer(identityDbConn, sqlOptions => sqlOptions.MigrationsAssembly(identityAssemblyName)))
    .AddAspNetIdentity<ApplicationUser>()
    .AddProfileService<CustomProfileService>();
    //.AddDeveloperSigningCredential(); // Please comments this if you are using private key.

builder.Services.AddTransient<IProfileService, CustomProfileService>();

#endregion

#region CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: ServerConstants.APP_CORS_POLICY,
        policy =>
        {
            policy.WithOrigins("https://localhost:3000", "http://localhost:3000", "https://localhost:8091")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
});

builder.Services.AddSingleton<ICorsPolicyService>((container) =>
{
    var logger = container.GetRequiredService<ILogger<DefaultCorsPolicyService>>();
    return new DefaultCorsPolicyService(logger)
    {
        AllowAll = true
    };
});
#endregion


builder.Services.AddAuthentication();

builder.Services.AddRazorPages();
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseIdentityServer();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors(ServerConstants.APP_CORS_POLICY);

app.UseAuthorization();

app.MapRazorPages().RequireAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


//Console.Clear();
//Log.Information("Seeding identity database");
//app.EnsureSeedData();
//Log.Information("Seeding identity database - COMPLETED");

//Console.ReadLine();
//return;

app.Run();
