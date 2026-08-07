using FluentValidation;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebApp.Customer.Client.Utilities;
using WebApp.Shared;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(_ =>
{
    var handler = new CookieHandler();
    return new HttpClient(handler) { BaseAddress = new Uri("https://localhost:7188/") };
});

builder.Services.AddValidatorsFromAssemblyContaining<AssemblyMarker>(lifetime: ServiceLifetime.Singleton);

await builder.Build().RunAsync();
