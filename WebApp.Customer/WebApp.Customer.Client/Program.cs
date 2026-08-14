using FluentValidation;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebApp.Customer.Client.Auth;
using WebApp.Customer.Client.Extensions;
using WebApp.Customer.Client.Services.Implementations;
using WebApp.Customer.Client.Services.Interfaces;
using WebApp.Customer.Client.Utilities;
using WebApp.Shared;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddTransient<WasmCookieHandler>();

builder.Services.AddApiClientServices<WasmCookieHandler>("https://localhost:7188/");
builder.Services.AddSharedClientServices();

await builder.Build().RunAsync();
