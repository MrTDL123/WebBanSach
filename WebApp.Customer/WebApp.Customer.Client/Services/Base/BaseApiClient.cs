using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System.Net;
using System.Net.Http.Json;
using WebApp.Shared.Dtos.Common;

namespace WebApp.Customer.Client.Services.Base
{
    public abstract class BaseApiClient
    {
        protected readonly HttpClient _httpClient;
        protected readonly ICookieProvider? _cookieProvider;
        protected readonly AntiforgeryStateProvider? _antiforgeryStateProvider;
        protected BaseApiClient(
            HttpClient httpClient,
            ICookieProvider? cookieProvider,
            AntiforgeryStateProvider? antiforgeryStateProvider)
        {
            _httpClient = httpClient;
            _cookieProvider = cookieProvider;
            _antiforgeryStateProvider = antiforgeryStateProvider;
        }

        protected virtual HttpRequestMessage CreateRequest(HttpMethod method, string uri, HttpContent? content = null)
        {
            var request = new HttpRequestMessage(method, uri);
            if (content != null)
            {
                request.Content = content;
            }

            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            var cookieValue = _cookieProvider?.GetAuthCookie();
            if (!string.IsNullOrEmpty(cookieValue))
            {
                request.Headers.Add("Cookie", $".WebBanSach.Auth={cookieValue}");
            }

            // Đính kèm Antiforgery Token cho các giao thức có khả năng thay đổi dữ liệu
            if (method != HttpMethod.Get && method != HttpMethod.Head && method != HttpMethod.Options)
            {
                var token = _antiforgeryStateProvider?.GetAntiforgeryToken();
                if (token != null && !string.IsNullOrEmpty(token.Value))
                {
                    request.Headers.Add("RequestVerificationToken", token.Value);
                }
            }

            return request;
        }

        #region Helper Methods

        protected Task<ApiResponse<T>> GetAsync<T>(string uri)
        {
            var request = CreateRequest(HttpMethod.Get, uri);
            return SendRequestAsync<T>(request);
        }

        protected Task<ApiResponse<TResponse>> PostAsJsonAsync<TRequest, TResponse>(string uri, TRequest data)
        {
            var content = JsonContent.Create(data);
            var request = CreateRequest(HttpMethod.Post, uri, content);
            return SendRequestAsync<TResponse>(request);
        }

        protected Task<ApiResponse<T>> PostAsync<T>(string uri, HttpContent? content = null)
        {
            var request = CreateRequest(HttpMethod.Post, uri, content);
            return SendRequestAsync<T>(request);
        }

        protected Task<ApiResponse<TResponse>> PutAsJsonAsync<TRequest, TResponse>(string uri, TRequest data)
        {
            var content = JsonContent.Create(data);
            var request = CreateRequest(HttpMethod.Put, uri, content);
            return SendRequestAsync<TResponse>(request);
        }

        protected Task<ApiResponse<T>> DeleteAsync<T>(string uri)
        {
            var request = CreateRequest(HttpMethod.Delete, uri);
            return SendRequestAsync<T>(request);
        }

        #endregion

        protected async Task<ApiResponse<T>> SendRequestAsync<T>(HttpRequestMessage request)
        {
            try
            {
                using (request)
                {
                    var response = await _httpClient.SendAsync(request);

                    // 1. Xử lý trường hợp hết hạn Session / Cookie (401)
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        return ApiResponse<T>.FailureResult("Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại.");
                    }

                    // 2. Xử lý trường hợp không có quyền (403)
                    if (response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        return ApiResponse<T>.FailureResult("Bạn không có quyền thực hiện thao tác này.");
                    }

                    // 3. Đọc dữ liệu JSON trả về trước (kể cả khi API trả mã 400 Bad Request với thông tin lỗi chi tiết)
                    try
                    {
                        var result = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
                        if (result != null)
                        {
                            return result;
                        }
                    }
                    catch
                    {
                        // Bỏ qua nếu response không phải JSON
                    }

                    // 4. Fallback nếu không đọc được JSON body
                    if (!response.IsSuccessStatusCode)
                    {
                        return ApiResponse<T>.FailureResult($"Máy chủ phản hồi lỗi (HTTP {(int)response.StatusCode}).");
                    }

                    return ApiResponse<T>.FailureResult("Không thể đọc dữ liệu phản hồi từ máy chủ.");
                }
            }
            catch (OperationCanceledException)
            {
                return ApiResponse<T>.FailureResult("Yêu cầu đã bị hủy hoặc quá thời gian chờ.");
            }
            catch (HttpRequestException ex)
            {
                return ApiResponse<T>.FailureResult($"Không thể kết nối đến máy chủ: {ex.Message}");
            }
            catch (Exception ex)
            {
                return ApiResponse<T>.FailureResult($"Lỗi hệ thống: {ex.Message}");
            }
        }
    }
}