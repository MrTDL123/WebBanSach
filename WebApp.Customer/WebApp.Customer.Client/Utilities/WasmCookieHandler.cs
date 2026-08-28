using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace WebApp.Customer.Client.Utilities
{
    public class WasmCookieHandler : DelegatingHandler
    {
        private readonly NavigationManager _navigationManager;
        public WasmCookieHandler(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // Yêu cầu trình duyệt gắn cookie vào header mỗi request
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            request.Headers.Add("X-Requested-With", ["XMLHttpRequest"]);

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var currentUri = _navigationManager.Uri;

                if (!currentUri.Contains("/login", StringComparison.OrdinalIgnoreCase))
                {
                    var returnUrl = Uri.EscapeDataString(currentUri);
                    _navigationManager.NavigateTo($"/login?returnUrl={returnUrl}", forceLoad: true);
                }
            }

            return response;
        }

    }
}
