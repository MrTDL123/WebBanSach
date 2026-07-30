using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebApp.Customer.Client.Utilities;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(_ =>
{
    var handler = new CookieHandler();
    return new HttpClient(handler) { BaseAddress = new Uri("https://localhost:7188/") };
});

await builder.Build().RunAsync();
