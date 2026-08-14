using FluentValidation;
using WebApp.Shared.Dtos.User;
using SixLabors.ImageSharp;
//using static System.Net.Mime.MediaTypeNames;

namespace WebApp.Shared.Validations.User
{
    public class UpdateProfileDtoValidator : AbstractValidator<UpdateProfileDto>
    {
        private const int MaxBase64ImageLength = 7 * 1024 * 1024;

        public UpdateProfileDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Họ tên không được để trống")
                .MaximumLength(150).WithMessage("Họ tên tối đa 150 ký tự");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^(0\d{9})$").WithMessage("Số điện thoại không hợp lệ (cần 10 chữ số, bắt đầu bằng 0)")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTime.Today).WithMessage("Ngày sinh không hợp lệ")
                .When(x => x.DateOfBirth.HasValue);

            // Validation kiểm tra dung lượng Avatar upload (Max 5MB)
            When(x => !string.IsNullOrEmpty(x.AvatarBase64), () =>
            {
                RuleFor(x => x.AvatarBase64)
                    .Must(base64 => base64!.Length <= MaxBase64ImageLength)
                    .WithMessage("Kích thước ảnh đại diện không được vượt quá 5MB");
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
