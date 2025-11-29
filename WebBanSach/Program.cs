using Media.DataAccess.Repository;
using Media.DataAccess.Repository.IRepository;
using Media.Models;
using Media.Service;
using Media.Service.IServices;
using Media.Utility;
using Meida.DataAccess.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using OfficeOpenXml;

// Thiết lập license EPPlus cho toàn ứng dụng
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.


builder.Services.AddDbContext<ApplicationDbContext>(options=>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
//ConfigureApplicationCookie buộc phải viết đằng sau AddIdentity
//Chỉnh lại điều hướng của trang khi người dùng đăng nhập vào trang không thuộc thẩm quyền
builder.Services.AddIdentity<TaiKhoan, IdentityRole>(options =>
{
    // Cấu hình đơn giản cho mật khẩu
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3; // chỉ cần tối thiểu 3 ký tự
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();
// ===== COOKIE CONFIGURATION (Ghi nhớ đăng nhập) =====
builder.Services.ConfigureApplicationCookie(options =>
{
    // Đường dẫn mặc định
    options.LoginPath = "/Customer/KhachHang/DangNhap";
    options.LogoutPath = "/Customer/KhachHang/DangXuat";
    options.AccessDeniedPath = "/Customer/KhachHang/AccessDenied";

    // 🔐 Cấu hình ghi nhớ đăng nhập
    options.ExpireTimeSpan = TimeSpan.FromDays(1);  // Cookie tồn tại 30 ngày
    options.SlidingExpiration = true;                // Tự động gia hạn nếu người dùng hoạt động
    options.Cookie.HttpOnly = true;                  // Chống truy cập cookie từ JS
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Bảo mật khi HTTPS

    options.Events = new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents
    {
        // Sự kiện: Khi chưa đăng nhập (401 Unauthorized)
        OnRedirectToLogin = ctx =>
        {
            var adminLoginPath = "/Admin/User/DangNhap";
            var requestPath = ctx.Request.Path;

            // Kiểm tra xem URL người dùng đang vào có bắt đầu bằng "/Admin" không
            if (requestPath.StartsWithSegments("/Admin")
                && !requestPath.StartsWithSegments(adminLoginPath))
            {
                // Nếu là Admin, chuyển hướng sang trang Login của Admin
                var returnUrl = requestPath + ctx.Request.QueryString;
                ctx.Response.Redirect($"/Admin/User/DangNhap?ReturnUrl={Uri.EscapeDataString(returnUrl)}");
            }
            else
            {
                // Nếu là Customer (hoặc các trang khác), dùng cơ chế mặc định
                ctx.Response.Redirect(ctx.RedirectUri);
            }

            return Task.CompletedTask;
        },

        // Sự kiện: Khi đã đăng nhập nhưng không đủ quyền (403 Forbidden)
        OnRedirectToAccessDenied = ctx =>
        {
            var requestPath = ctx.Request.Path;

            if (requestPath.StartsWithSegments("/Admin"))
            {
                // Nếu đang ở Admin mà bị cấm -> Sang trang cấm của Admin
                ctx.Response.Redirect("/Admin/User/AccessDenied");
            }
            else
            {
                // Mặc định
                ctx.Response.Redirect(ctx.RedirectUri);
            }

            return Task.CompletedTask;
        }
    };
});
builder.Services.AddScoped<IUnitOfWork, UnitOfwork>();
builder.Services.AddMemoryCache();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<ISlugService, SlugService>();
builder.Services.AddScoped<IViewRenderService, ViewRenderService>();
builder.Services.AddHttpContextAccessor();
//builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddHttpClient<LocationService>(client =>
{
    // Base URL cho API
    client.BaseAddress = new Uri("https://provinces.open-api.vn/api/");
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    // Bỏ qua xác thực SSL (chỉ nên dùng cho môi trường phát triển)
    return new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
    };
});
builder.Services.AddSession();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IGioHangService, GioHangService>();


builder.Services.AddAuthentication().AddFacebook(opt =>
{
    opt.ClientId = "2595670757499848";
    opt.ClientSecret = "d49eb165fa95713bcf206dde65bb3e68";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();



async Task CreateRolesAsync(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roleNames = { SD.Role_Admin, SD.Role_Customer, SD.Role_Company, SD.Role_Employee };

    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}

// Gọi tạo role khi ứng dụng khởi chạy
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await CreateRolesAsync(services);
}

app.MapControllerRoute(
    name: "AdminRoute",
    pattern: "Admin", // Đường dẫn bạn muốn bắt
    defaults: new { area = "Admin", controller = "Dashboard", action = "Index" }
);

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "chude",
    pattern: "chude/{*path}",
    defaults: new { area = "Customer", controller = "Home", action = "SachTheoChuDe" });


app.Run();
