using FluentValidation;
using WebApp.Shared.Dtos.Management.ContentAndMarketing;
using SixLabors.ImageSharp;

namespace WebApp.Shared.Validations.Management.ContentAndMarketing
{
    public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
    {
        // 5MB trong Base64 tương đương khoảng 7,000,000 ký tự
        private const int MaxBase64ImageLength = 7 * 1024 * 1024;
        public UpdateProductDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("Mã sản phẩm không hợp lệ");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Tên sách không được để trống")
                .MaximumLength(255).WithMessage("Tên sách tối đa 255 ký tự");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Giá bán phải lớn hơn 0");

            RuleFor(x => x.DiscountPercent)
                .InclusiveBetween(0, 100).WithMessage("Phần trăm giảm giá phải từ 0 đến 100");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Số lượng tồn kho không được âm");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Vui lòng chọn danh mục sách");

            RuleFor(x => x.AuthorId)
                .GreaterThan(0).WithMessage("Vui lòng chọn tác giả");

            RuleFor(x => x.PublisherId)
                .GreaterThan(0).WithMessage("Vui lòng chọn nhà xuất bản");

            // Validation kiểm tra dung lượng ảnh upload (Max 5MB)
            When(x => !string.IsNullOrEmpty(x.MainImageBase64), () =>
            {
                RuleFor(x => x.MainImageBase64)
                    .Must(base64 => base64!.Length <= MaxBase64ImageLength)
                    .WithMessage("Kích thước ảnh bìa chính không được vượt quá 5MB")
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
