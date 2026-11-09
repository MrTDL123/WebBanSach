using System;
using System.Collections.Generic;
using System.Text;

namespace WebApp.Shared.Utilities
{
    /// <summary>
    /// Kiểm tra định dạng ảnh thông qua các chữ ký số 
    /// </summary>
    public static class ImageSignatureHelper
    {
        private static readonly byte[] PngHeader = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
        private static readonly byte[] JpegHeader = [0xFF, 0xD8, 0xFF];

        // 1 file ảnh Webp có 12 bytes (Riff là 4 byte và 4 byte cuối mới là byte của Webp)
        private static readonly byte[] RiffHeader = [0x52, 0x49, 0x46, 0x46];
        private static readonly byte[] WebpHeader = [0x57, 0x45, 0x42, 0x50];

        public static bool IsValidImageBase64(string? base64String)
        {
            if (string.IsNullOrWhiteSpace(base64String)) return false;

            var span = base64String.AsSpan().Trim();
            // Bỏ qua tiền tố (data:image/png;base64,)
            int commaIndex = span.IndexOf(',');
            if (commaIndex >= 0)
            {
                var prefix = span.Slice(0, commaIndex);

                if (!prefix.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase) ||
                    !prefix.EndsWith(";base64", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
                span = span.Slice(commaIndex + 1);
            }

            // Cần tối thiểu 16 bytes nhị phân => Tối đa 24 ký tự Base64 (4 ký tự Base64 = 3 bytes)
            if (span.Length < 16) return false;

            // Ta cần header của file ảnh Base64 là 32 ký tự
            int charsToTake = Math.Min(span.Length, 32);
            // Vì Convert.TryFromBase64Chars yêu cầu chuỗi đầu vào chia hết cho 4
            charsToTake -= (charsToTake % 4);
            if (charsToTake < 16) return false;

            Span<byte> buffer = stackalloc byte[24];
            if (!Convert.TryFromBase64Chars(span.Slice(0, charsToTake), buffer, out int bytesWritten) || bytesWritten < 12)
            {
                return false;
            }

            return IsValidImageBytes(buffer.Slice(0, bytesWritten));
        }

        public static bool IsValidImageBytes(ReadOnlySpan<byte> bytes)
        {
            // Kiểm tra PNG (8 bytes)
            if (bytes.Length >= 8 && bytes.Slice(0, 8).SequenceEqual(PngHeader))
                return true;

            // Kiểm tra JPEG (3 bytes)
            if (bytes.Length >= 3 && bytes.Slice(0, 3).SequenceEqual(JpegHeader))
                return true;

            // Kiểm tra WEBP (12 bytes: RIFF****WEBP)
            if (bytes.Length >= 12 &&
                bytes.Slice(0, 4).SequenceEqual(RiffHeader) && 
                bytes.Slice(8, 4).SequenceEqual(WebpHeader))
            {
                return true;
            }

            return false;
        }
    }
}
