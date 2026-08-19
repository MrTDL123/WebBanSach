using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebApp.Customer.Client.Extensions;
using WebApp.Customer.Client.Utilities;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddTransient<WasmCookieHandler>();
builder.Services.AddTransient<AntiforgeryHandler>();

var apiBaseAddress = "https://localhost:7188/";

builder.Services.AddApiClientServices<WasmCookieHandler>(apiBaseAddress, isAssemblyRenderMode: true);
builder.Services.AddSharedClientServices();

await builder.Build().RunAsync();
