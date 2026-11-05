using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebApp.Customer.Client.Extensions;
using WebApp.Customer.Client.Services.Base;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddSharedClientServices();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

// Đăng ký giải mã trạng thái xác thực từ Html persistent states
builder.Services.AddAuthenticationStateDeserialization();

// Đăng ký ClientCookieProvider cho WASM Runtime (trình duyệt tự gắn cookie)
builder.Services.AddScoped<ICookieProvider, ClientCookieProvider>();

var apiBaseAddress = "https://localhost:7188/";
builder.Services.AddApiClientServices(apiBaseAddress);

await builder.Build().RunAsync();
