using System.Data;
using WebApp.Api.Services.Interfaces;

namespace WebApp.Api.Services.Implementations
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _environment;
        public FileStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveBase64ImageAsync(string base64Data, string subfolder, string prefix)
        {
            // Xử lý lưu File Ảnh An Toàn (Nếu có gửi dữ liệu Base64)
            var cleanBase64 = base64Data.Contains(",") ? base64Data.Split(',')[1] : base64Data;
            var bytes = Convert.FromBase64String(cleanBase64);

            // Kiểm tra dung lượng tối đa 5MB
            if (bytes.Length > 5 * 1024 * 1024)
            {
                throw new InvalidOperationException("Kích thước ảnh không được vượt quá 5MB");
            }

            // Kiểm tra Magic Bytes nhị phân (Chống upload file độc giả dạng ảnh)
            if (!IsValidImageMagicBytes(bytes))
            {
                throw new InvalidOperationException("Định dạng của file không hợp lệ.");
            }

            var webRoot = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
            var folderPath = Path.Combine(webRoot, subfolder);
            if (!Directory.Exists(folderPath)) 
            {
                Directory.CreateDirectory(folderPath);
            }

            // Đổi tên file thành GUID ngẫu nhiên (Chống Path Traversal)
            /*
                Ý nghĩa: Tạo ra một tên file hoàn toàn ngẫu nhiên và duy nhất 
                    bằng mã GUID.

                ⚠️ Nếu thiếu: Nếu dùng tên file do người dùng gửi lên, 
                    hacker có thể đặt tên file là ../../appsettings.json 
                    hoặc ../../Program.cs. Khi Server lưu file, nó sẽ 
                    ghi đè và phá hủy mã nguồn hoặc làm lộ mật khẩu Database của bạn!
             */
            var fileName = $"{prefix}_{Guid.NewGuid():N}.jpg";
            var filePath = Path.Combine(folderPath, fileName);
            await File.WriteAllBytesAsync(filePath, bytes);
            return $"/{subfolder.Replace("\\", "/")}/{fileName}";
        }

        // 🔥 BƯỚC DỌN RÁC TỰ ĐỘNG: Xóa ảnh cũ khỏi đĩa cứng (nếu đã từng có ảnh trước đó)
        public void DeletePhysicalFile(string? relativeUrl)
        {
            if (string.IsNullOrEmpty(relativeUrl)) return;

            var webRoot = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
            var fullPath = Path.Combine(webRoot, relativeUrl.TrimStart('/'));
            if (File.Exists(fullPath)) File.Delete(fullPath);
        }

        public void DeletePhysicalFolder(string? relativeFolderPath)
        {
            if (string.IsNullOrEmpty(relativeFolderPath)) return;

            var webRoot = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
            var fullPath = Path.Combine(webRoot, relativeFolderPath.TrimStart('/'));
            if (Directory.Exists(fullPath)) Directory.Delete(fullPath, recursive: true);
            // recursive: true nghĩa là xóa luôn cả thư mục và tất cả file bên trong!
        }


        // Hàm kiểm tra Chữ ký Nhị phân (Magic Bytes) của ảnh thật
        /*
            Ý nghĩa: Soi 2 đến 4 byte đầu tiên của tệp nhị phân. 
                Bất kỳ file ảnh nào trên thế giới cũng bắt đầu bằng chữ ký số nhị phân:
                .)File JPG bắt buộc phải bắt đầu bằng: FF D8 FF
                .)File PNG bắt buộc phải bắt đầu bằng: 89 50 4E 47
                .)File WEBP bắt buộc phải bắt đầu bằng: RIFF...WEBP

            ⚠️ Nếu thiếu (LỖ HỔNG BẢO MẬT CỰC NGUY HIỂM): Hacker viết một con 
                virus mã độc (ví dụ file trojan.exe hoặc script shell.php/asp), 
                sau đó đổi tên đuôi file thành avatar.jpg để lừa bộ lọc. Nếu không soi 
                Magic Bytes, Server sẽ lưu file virus này vào wwwroot. Sau đó 
                hacker chỉ cần kích hoạt file đó là chiếm toàn quyền kiểm soát máy chủ của bạn!
         */
        private bool IsValidImageMagicBytes(byte[] bytes)
        {
            if (bytes.Length < 4) return false;
            // JPEG/JPG: FF D8 FF
            var isJpeg = bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF;
            // PNG: 89 50 4E 47
            var isPng = bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47;
            // WEBP: RIFF...WEBP
            var isWebp = bytes[0] == 0x52 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x46;
            return isJpeg || isPng || isWebp;
        }
    }
}
