using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.AspNetCore.DataProtection;
using WebApp.Customer.Client.Auth;
using WebApp.Customer.Client.Extensions;
using WebApp.Customer.Client.Pages;
using WebApp.Customer.Client.Services.Implementations;
using WebApp.Customer.Client.Services.Interfaces;
using WebApp.Customer.Components;
using WebApp.Customer.Utilities;
using WebApp.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<ServerCookieHandler>();

var apiBaseAddress = builder.Configuration["ApiBaseAddress"] ?? "https://localhost:7188/";

builder.Services.AddApiClientServices<ServerCookieHandler>(apiBaseAddress, isAssemblyRenderMode: false);

builder.Services.AddSharedClientServices();

// TODO: CẦN CHUYỂN SANG X509Certificate ĐỂ MÃ HÓA COOKIE PHÙ HỢP TẤT CẢ HỆ ĐIỀU HÀNH
var keysPath = builder.Configuration["DataProtection:KeysPath"]
    ?? (OperatingSystem.IsWindows()
        ? @"C:\SharedKeys\WebBanSach"
        : "/app/SharedKeys/WebBanSach");

var keysDir = new DirectoryInfo(keysPath);
if (!keysDir.Exists)
{
    keysDir.Create();
}

var dataProtection = builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(keysDir)
        .SetApplicationName("SharedCookieWebBanSach");

if (OperatingSystem.IsWindows())
{
    dataProtection.ProtectKeysWithDpapi();
}

builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(360);
});
builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
    options.HttpsPort = 7035;
});

// Cấu hình cơ chế (authentication scheme) để AuthenticationHandler ưu tiên cookie khi xác thực
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.Name = ".WebBanSach.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(WebApp.Customer.Client._Imports).Assembly);

app.Run();
