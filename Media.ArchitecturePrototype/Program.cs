using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Media.ArchitecturePrototype
{
    class Program
    {
        private static readonly string BaseUrl = "https://localhost:7014";
        private static readonly string DbConnectionString = "Server=DUYLONG\\SQLEXPRESS;Database=WebBanSach;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";
        private static readonly CookieContainer cookieContainer = new CookieContainer();
        private static readonly HttpClient client = new HttpClient(new HttpClientHandler
        {
            CookieContainer = cookieContainer,
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true,
            AllowAutoRedirect = false
        });
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine($"Target System: {BaseUrl}");
            Console.WriteLine("------------------------------------------------------------------");

            Console.WriteLine("[PHASE 1] XÁC THỰC MÃ HÓA LƯU TRỮ TẠI DATABASE");
            await ValidateDatabaseEncryption();
            Console.WriteLine("------------------------------------------------------------------");

            // PHASE 2: XÁC THỰC QUYỀN TRUY CẬP (ACCESS CONTROL)
            Console.WriteLine("[PHASE 2] XÁC THỰC BẢO VỆ ĐIỀU HƯỚNG & QUYỀN (ACCESS CONTROL)");

            // Kịch bản 1: Kẻ ẩn danh cố vào trang Quản lý Sách
            await TestAnonymousAccess();

            // Kịch bản 2: Tài khoản KHÔNG ĐỦ QUYỀN (Customer) cố đăng nhập Admin
            await TestUnauthorizedLogin("lsluongtranduy@gmail.com", "12345");

            // Kịch bản 3: Tài khoản HỢP LỆ (Admin) đăng nhập
            await TestAuthorizedLogin("jackacevn@gmail.com", "12345");

            Console.WriteLine("------------------------------------------------------------------");
            Console.WriteLine("Xác thực kiến trúc bảo mật hoàn tất. Nhấn phím bất kỳ để thoát...");
            Console.ReadKey();
        }

        private static async Task ValidateDatabaseEncryption()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DbConnectionString))
                {
                    await conn.OpenAsync();
                    Console.WriteLine("Kết nối Database thành công");
                    
                    string query = "SELECT TOP 1 UserName, PasswordHash FROM [WebBanSach].[dbo].[AspNetUsers]";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string user = reader.GetString(0);
                            string hashPassword = reader.GetString(1);
                            Console.WriteLine($"Kiểm tra dữ liệu tài khoản: {user}");
                            Console.WriteLine($"   -> Dữ liệu băm (Hash) lưu trong DB: {hashPassword.Substring(0, 30)}...");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LỖI PHASE 1] Không thể kết nối hoặc đọc CSDL: {ex.Message}");
            }
        }

        private static async Task TestAnonymousAccess()
        {
            string targetUrl = $"{BaseUrl}/Admin/Book/QuanLySach";
            var response = await client.GetAsync(targetUrl);
            Console.WriteLine("Kịch bản 1: Truy cập ẩn danh vào Admin Area");
            Console.WriteLine($"   -> Target: {targetUrl}");

            if (response.StatusCode == HttpStatusCode.Redirect || response.StatusCode == HttpStatusCode.Found)
            {
                var redirectUrl = response.Headers.Location.ToString();
                if (redirectUrl.Contains("DangNhap"))
                {
                    Console.WriteLine($"   -> Kết quả: CHẶN ĐỨNG | Trạng thái: Chuyển hướng ({response.StatusCode}) về {redirectUrl}");
                }
            }
            else
            {
                Console.WriteLine($"   -> Kết quả: THẤT BẠI | Trạng thái: {response.StatusCode} (Không bị chặn như mong đợi)");
            }
        }

        static async Task TestUnauthorizedLogin(string username, string password)
        {
            Console.WriteLine($"Kịch bản 2: Tài khoản KHÔNG ĐỦ QUYỀN [{username}] cố đăng nhập Admin...");
            string token = await GetAntiForgeryToken();

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("UserName", username),
                new KeyValuePair<string, string>("MatKhau", password),
                new KeyValuePair<string, string>("__RequestVerificationToken", token)
            });

            var response = await client.PostAsync($"{BaseUrl}/Admin/User/DangNhap", content);
            string htmlResponse = await response.Content.ReadAsStringAsync();

            string decodedHtml = System.Net.WebUtility.HtmlDecode(htmlResponse);

            if (decodedHtml.Contains("Tài khoản không có quyền hạn"))
            {
                Console.WriteLine("   -> Kết quả: CHẶN ĐỨNG | Hệ thống từ chối Session và báo lỗi quyền hạn.");
            }
            else
            {
                Console.WriteLine($"   -> Kết quả: KHÔNG RÕ RÀNG | Trạng thái mã HTTP: {response.StatusCode}");
            }
        }

        static async Task TestAuthorizedLogin(string username, string password)
        {
            Console.WriteLine($"Kịch bản 3: Tài khoản HỢP LỆ [{username}] đăng nhập Admin...");
            string token = await GetAntiForgeryToken();

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("UserName", username),
                new KeyValuePair<string, string>("MatKhau", password),
                new KeyValuePair<string, string>("__RequestVerificationToken", token)
            });

            var response = await client.PostAsync($"{BaseUrl}/Admin/User/DangNhap", content);

            if (response.StatusCode == HttpStatusCode.Redirect || response.StatusCode == HttpStatusCode.Found)
            {
                Console.WriteLine($"          -> Đăng nhập thành công! Hệ thống chuyển hướng tới: {response.Headers.Location}");

                // Trình duyệt sẽ tự động gọi tiếp trang Quản lý sách bằng Session vừa lấy được
                var authorizedClient = new HttpClient(new HttpClientHandler
                {
                    CookieContainer = cookieContainer, // Dùng lại Cookie (Session) vừa được cấp
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                });

                Console.WriteLine("   -> Tiến hành truy cập dữ liệu: /Admin/Book/QuanLySach");
                var secureResponse = await authorizedClient.GetAsync($"{BaseUrl}/Admin/Book/QuanLySach");

                Console.WriteLine($"   -> Kết quả: HỢP LỆ | Trạng thái: {(int)secureResponse.StatusCode} {secureResponse.ReasonPhrase}");
            }
            else
            {
                Console.WriteLine($"   -> Kết quả: ĐĂNG NHẬP THẤT BẠI | Trạng thái: {response.StatusCode}");
            }
        }

        // Hàm tiện ích: Trích xuất __RequestVerificationToken từ trang đăng nhập
        static async Task<string> GetAntiForgeryToken()
        {
            var response = await client.GetAsync($"{BaseUrl}/Admin/User/DangNhap");
            string html = await response.Content.ReadAsStringAsync();

            Match match = Regex.Match(html, @"<input name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)""");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return string.Empty;
        }
    }
}