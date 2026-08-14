using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace WebApp.Customer.Client.Utilities
{
    public class WasmCookieHandler : DelegatingHandler
    {
        public WasmCookieHandler() {}

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            return base.SendAsync(request, cancellationToken);
        }

    }
}
