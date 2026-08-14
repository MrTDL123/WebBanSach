using System.Net.Http.Json;
using WebApp.Customer.Client.Services.Interfaces;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Customer.Auth;

namespace WebApp.Customer.Client.Services.Implementations
{
    public class AuthClientService : IAuthClientService
    {
        private readonly HttpClient _httpClient;

        public AuthClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<AuthResponseDto>> GetCurrentUserAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/auth/me");
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponse<AuthResponseDto>.FailureResult("Chưa đăng nhập.");
                }
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDto>>();
                return result ?? ApiResponse<AuthResponseDto>.FailureResult("Lỗi đọc dữ liệu người dùng.");
            }
            catch
            {
                return ApiResponse<AuthResponseDto>.FailureResult("Không thể xác thực phiên làm việc.");
            }
        }

        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(CustomerLoginDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/customer-login", dto);
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDto>>();
                return result ?? ApiResponse<AuthResponseDto>.FailureResult("Lỗi khi đọc phản hồi từ Server.");
            }
            catch (Exception ex) 
            {
                return ApiResponse<AuthResponseDto>.FailureResult($"Lỗi kết nối máy chủ: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> LogoutAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync("api/auth/logout", null);
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
                return result ?? ApiResponse<bool>.SuccessResult(true);
            }
            catch
            {
                return ApiResponse<bool>.FailureResult("Đã có lỗi xảy ra khi đăng xuất.");
            }
        }
    }
}
