using FluentValidation;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Text;
using WebApp.Shared.Dtos.Management.ContentAndMarketing;

namespace WebApp.Shared.Validations.Management.ContentAndMarketing
{
    public class CreateFreeBookDtoValidator : AbstractValidator<CreateFreeBookDto>
    {
        private const int MaxBase64ImageLength = 7 * 1024 * 1024; // ~5MB
        public CreateFreeBookDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Tên sách 0 đồng không được để trống")
                .MaximumLength(255).WithMessage("Tên sách tối đa 255 ký tự");
            RuleFor(x => x.AuthorName)
                .NotEmpty().WithMessage("Tên tác giả không được để trống")
                .MaximumLength(150).WithMessage("Tên tác giả tối đa 150 ký tự");
            RuleFor(x => x.Condition)
                .NotEmpty().WithMessage("Vui lòng nhập tình trạng sách (ví dụ: Mới 100%, Cũ 90%...)")
                .MaximumLength(100).WithMessage("Tình trạng sách tối đa 100 ký tự");
            // Kiểm tra ảnh bìa upload (nếu có chọn ảnh)
            When(x => !string.IsNullOrEmpty(x.CoverImageBase64), () =>
            {
                RuleFor(x => x.CoverImageBase64)
                    .Must(base64 => base64!.Length <= MaxBase64ImageLength)
                    .WithMessage("Kích thước ảnh bìa không được vượt quá 5MB")
                    .Must(BeAValidImage).WithMessage("Ảnh đại diện không hợp lệ hoặc không đúng định dạng (chỉ chấp nhận PNG, JPG, JPEG, WEBP)");
            });
        }

        private bool BeAValidImage(string? base64String)
        {
            if (string.IsNullOrEmpty(base64String)) return true;

            try
            {
                // 1. Kiểm tra tiền tố Header Base64 chuẩn của Ảnh
                if (!base64String.StartsWith("data:image/png;base64,", StringComparison.OrdinalIgnoreCase) &&
                    !base64String.StartsWith("data:image/jpeg;base64,", StringComparison.OrdinalIgnoreCase) &&
                    !base64String.StartsWith("data:image/jpg;base64,", StringComparison.OrdinalIgnoreCase) &&
                    !base64String.StartsWith("data:image/webp;base64,", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                // 2. Tách chuỗi Base64 và Decode thử ra mảng Byte
                var parts = base64String.Split(',');
                if (parts.Length < 2) return false;

                byte[] imageBytes = Convert.FromBase64String(parts[1]);

                // 3. Đọc thử cấu trúc File bằng ImageSharp
                // Nếu là file mã độc .exe hay .php fake đuôi, ImageSharp sẽ throw Exception -> Rơi vào catch -> Return false!
                using var image = Image.Load(imageBytes);
                return true;
            }
            catch
            {
                return false; // Decode lỗi hoặc không đúng định dạng ảnh -> Từ chối ngay lập tức!
            }
        }
    }
}
