using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using WebApp.Api.Data;
using WebApp.Api.Endpoints;
using WebApp.Api.Entities;
using WebApp.Api.Hubs;
using WebApp.Api.Services.Implementations;
using WebApp.Api.Services.Interfaces;
using WebApp.Api.Utilities;
using WebApp.Shared;

var builder = WebApplication.CreateBuilder(args);

// Tránh việc gọi Attribute [Authorize] lên các Controller
// Lưu ý: cần [AllowAnonymous] cho các endpoint mà không cần đăng nhập
builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build());

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Đăng ký Cors 
const string AllowBlazorClientsPolicy = "AllowBlazorClients";
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins").Get<string[]>() ??
    new[] { "https://localhost:7135", "https://localhost:7035" };

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowBlazorClientsPolicy, policy =>
        {
            policy.WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials(); // Cho phép gửi Cookie và SignalR WebSocket từ các Blazor chạy ở InteractiveServer
        });
});

// Đăng ký và sử dụng Cookie
builder.Services.AddIdentity<User, Role>(options =>
{
    // Cấu hình mật khẩu
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;

    // Tránh bot Brute-Force mật khẩu
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình tài khoản
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Cấu hình thời gian kiểm tra định kỳ Security Stamp cho mỗi Circut
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.FromMinutes(10);
});


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

// Đăng ký Antiforgery để đọc header
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "RequestVerificationToken";
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = ".WebBanSach.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;

    // Trả về mã 403/401 thay vì Redirect
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Cấu hình Authentication kết hợp Cookie và JwtBearer thông qua PolicyScheme
const string SmartAuthScheme = "SmartAuthScheme";
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? new JwtSettings();
var jwtKeyBytes = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = SmartAuthScheme;
    options.DefaultAuthenticateScheme = SmartAuthScheme;
    options.DefaultChallengeScheme = SmartAuthScheme;
})
.AddPolicyScheme(SmartAuthScheme, "Bearer or Cookie", options =>
{
    options.ForwardDefaultSelector = context =>
    {
        // 1. Kiểm tra Authorization Header có tiền tố Bearer
        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return JwtBearerDefaults.AuthenticationScheme;
        }

        // 2. Kiểm tra query param access_token cho SignalR WebSocket
        if (context.Request.Path.StartsWithSegments("/hubs") &&
            context.Request.Query.ContainsKey("access_token"))
        {
            return JwtBearerDefaults.AuthenticationScheme;
        }

        // 3. Mặc định dùng Cookie Authentication cho Blazor
        return IdentityConstants.ApplicationScheme;
    };
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(jwtKeyBytes),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        NameClaimType = ClaimTypes.Name,
        RoleClaimType = ClaimTypes.Role
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddControllers();

// Thêm Context vào các request để Service có thể đọc Claims User
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Auth Services
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<ICustomerAuthService, CustomerAuthService>();
builder.Services.AddScoped<IAdminAuthService, AdminAuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IContentService, ContentService>();
builder.Services.AddScoped<ICustomerProductService, CustomerProductService>();
builder.Services.AddScoped<ICustomerCartService, CustomerCartService>();
builder.Services.AddScoped<ICustomerReturnService, CustomerReturnService>();
builder.Services.AddScoped<ICustomerAddressService, CustomerAddressService>();
builder.Services.AddScoped<ICustomerOrderService, CustomerOrderService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();

// Thêm Hub để cập nhập các update từ Admin
builder.Services.AddSignalR();

// Load các Validation được thiết lập ở WebApp.Shared
builder.Services.AddValidatorsFromAssemblyContaining<AssemblyMarker>();

// Cấu hình HSTS (HTTP Strict Transport Security) cho Production. Cấu hình giúp API chỉ nhận các Https request
builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(60);
});
// Tự động chuyển hóa toàn bộ Request Http thành Https
builder.Services.AddHttpsRedirection(options =>
{
    // Tránh lưu cache các dữ liệu chưa mã hóa từ Http trước khi được chuyển sang https
    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
    options.HttpsPort = 7188; // port https của API (cần phải sửa nếu lên Production)
});

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("Cookie");
    logging.ResponseHeaders.Add("Set-Cookie");
});


builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseHttpLogging();
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.UseDeveloperExceptionPage();

    // Tự động Seed dữ liệu mẫu khi chạy môi trường Development
    await DbInitializer.SeedAsync(app.Services);
}
else
{
    // Bật Hsts ở Production
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors(AllowBlazorClientsPolicy);

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapControllers();
app.MapHub<UpdateBroadcastHub>("/hubs/updates");
app.MapRevalidateStateEndpoints();// Thêm định tuyến cho việc kiểm tra trạng thái xác thực theo chu kỳ

app.Run();
