using System.Net;
using System.Net.Http.Json;
using WebApp.Admin.Utilities;
using WebApp.Shared.Dtos.Common;
namespace WebApp.Admin.Services.Base
{
    public abstract class BaseApiClient
    {
        protected readonly HttpClient _httpClient;
        protected readonly UserSessionState _userSessionState;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected BaseApiClient(
            HttpClient httpClient, 
            UserSessionState userSessionState,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _userSessionState = userSessionState;
            _httpContextAccessor = httpContextAccessor;
        }

        private HttpRequestMessage CreateRequest(HttpMethod method, string uri, HttpContent? content = null)
        {
            var request = new HttpRequestMessage(method, uri);
            if (content != null)
            {
                request.Content = content;
            }

            var cookieValue = _userSessionState.AuthCookieValue;
            if (string.IsNullOrEmpty(cookieValue))
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null && httpContext.Request.Cookies.TryGetValue(".WebBanSach.Auth", out var val))
                {
                    cookieValue = val;
                    _userSessionState.AuthCookieValue = val;
                }
            }

            if (!string.IsNullOrEmpty(cookieValue))
            {
                request.Headers.Add("Cookie", $".WebBanSach.Auth={cookieValue}");
            }

            return request;
        }

        #region Helper Methods

        protected Task<ApiResponse<T>> GetAsync<T>(string uri)
        {
            var request = CreateRequest(HttpMethod.Get, uri);
            return SendRequestAsync<T>(request);
        }

        protected Task<ApiResponse<T>> PostAsync<T>(string uri, HttpContent? content = null)
        {
            var request = CreateRequest(HttpMethod.Post, uri, content);
            return SendRequestAsync<T>(request);
        }

        protected Task<ApiResponse<TResponse>> PostAsJsonAsync<TRequest, TResponse>(string uri, TRequest data)
        {
            var content = JsonContent.Create(data);
            var request = CreateRequest(HttpMethod.Post, uri, content);
            return SendRequestAsync<TResponse>(request);
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

        /// <summary>
        /// Hàm lõi thực thi request và phân tích ApiResponse<T>
        /// </summary>
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

                    // 3. THỬ ĐỌC NỘI DUNG JSON TRẢ VỀ TRƯỚC (Kể cả khi StatusCode là HTTP 400 Bad Request)
                    try
                    {
                        var result = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
                        if (result != null)
                        {
                            // Trả về trọn vẹn Message & Errors chi tiết từ Backend WebApp.Api!
                            return result;
                        }
                    }
                    catch
                    {
                        // Nếu không đọc được JSON (ví dụ Server sập 500 HTML hoặc 502) thì bỏ qua nhảy xuống bước 4
                    }

                    // 4. Fallback dự phòng nếu không đọc được JSON body
                    if (!response.IsSuccessStatusCode)
                    {
                        return ApiResponse<T>.FailureResult($"Máy chủ phản hồi lỗi (HTTP {(int)response.StatusCode}).");
                    }

                    return ApiResponse<T>.FailureResult("Không thể đọc dữ liệu phản hồi từ máy chủ.");
                }
            }
            catch (OperationCanceledException)
            {
                // Bắt lỗi khi request bị hủy (ví dụ: timeout)
                return ApiResponse<T>.FailureResult("Yêu cầu đã bị hủy. Vui lòng thử lại.");
            }
            catch (HttpRequestException ex)
            {
                // Bắt lỗi không kết nối được tới API (mất mạng hoặc Server API sập)
                return ApiResponse<T>.FailureResult($"Không thể kết nối đến máy chủ: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Lỗi hệ thống không lường trước
                return ApiResponse<T>.FailureResult($"Lỗi hệ thống: {ex.Message}");
            }
        }
    }
}