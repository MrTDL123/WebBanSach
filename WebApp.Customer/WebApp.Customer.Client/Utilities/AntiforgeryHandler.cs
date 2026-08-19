using Microsoft.AspNetCore.Components.Forms;

namespace WebApp.Customer.Client.Utilities
{
    /// <summary>
    /// Hander để đính kèm Antiforgery Token mỗi request giúp chống các request giả mạo từ các web khác
    /// </summary>
    public class AntiforgeryHandler : DelegatingHandler
    {
        private readonly AntiforgeryStateProvider _antiforgeryStateProvider;
        public AntiforgeryHandler(AntiforgeryStateProvider antiforgeryStateProvider)
        {
            _antiforgeryStateProvider = antiforgeryStateProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Chỉ đính kèm các Request có khả năng thay đổi dữ liệu (POST, PUT, DELETE)
            if (request.Method != HttpMethod.Get && request.Method != HttpMethod.Head && request.Method != HttpMethod.Options)
            {
                var token = _antiforgeryStateProvider.GetAntiforgeryToken();
                if (token != null)
                {
                    request.Headers.Add("RequestVerificationToken", token.Value);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
