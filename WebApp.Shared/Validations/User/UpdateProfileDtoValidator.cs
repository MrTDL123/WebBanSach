using FluentValidation;
using WebApp.Shared.Dtos.User;
using WebApp.Shared.Utilities;

namespace WebApp.Shared.Validations.User
{
    public class UpdateProfileDtoValidator : AbstractValidator<UpdateProfileDto>
    {
        private const int MaxBase64ImageLength = 5 * 1024 * 1024;

        public UpdateProfileDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Họ tên không được để trống")
                .MaximumLength(150).WithMessage("Họ tên tối đa 150 ký tự");

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTime.UtcNow).WithMessage("Ngày sinh không hợp lệ")
                .When(x => x.DateOfBirth.HasValue);

            When(x => !string.IsNullOrEmpty(x.PhoneNumber), () =>
            {
                RuleFor(x => x.PhoneNumber!)
                    .Matches(@"^(0\d{9})$").WithMessage("Số điện thoại không hợp lệ (cần 10 chữ số, bắt đầu bằng 0)");
            });

            // Validation kiểm tra dung lượng Avatar upload (Max 5MB)
            When(x => !string.IsNullOrEmpty(x.AvatarBase64), () =>
            {
                RuleFor(x => x.AvatarBase64)
                    .Must(base64 => base64!.Length <= MaxBase64ImageLength)
                    .WithMessage("Kích thước ảnh đại diện không được vượt quá 5MB")
                    .Must(ImageSignatureHelper.IsValidImageBase64).WithMessage("Ảnh đại diện không hợp lệ hoặc không đúng định dạng (chỉ chấp nhận PNG, JPG, JPEG, WEBP)");
            });
        }
    }
}
