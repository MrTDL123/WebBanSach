using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using WebApp.Admin.Components;
using WebApp.Admin.Middlewares;
using WebApp.Admin.Utilities;
using WebApp.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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

// Cấu hình HSTS
builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(360);
});
builder.Services.AddHttpsRedirection(options =>
{
    // Tránh cache các dữ liệu chưa mã hóa trước khi chuyển sang https
    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
    options.HttpsPort = 7135; // Port HTTPS của WebApp.Admin
});

// Đăng ký Authentication Cookie cho Blazor Server
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.Name = ".WebBanSach.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax; // Dùng để chống CSRF
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorizationCore();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UserSessionState>();
builder.Services.AddTransient<CookieHandler>();


var apiBaseAddress = builder.Configuration["ApiBaseAddress"] ?? "https://localhost:7188/";

// Đăng ký HttpClient
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(apiBaseAddress);
})
.AddHttpMessageHandler<CookieHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient"));

builder.Services.AddValidatorsFromAssemblyContaining<AssemblyMarker>(lifetime: ServiceLifetime.Singleton);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Custom middleware để mỗi lần gửi, nhận request response thì phải đọc cookie
app.UseMiddleware<InitialSessionMiddleware>();

//  Chống việc người dùng gửi request có đính kèm cookie từ web giả mạo đến API
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

