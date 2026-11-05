using FluentValidation;
using WebApp.Shared.Dtos.Customer.Order;
using WebApp.Shared.Utilities;

namespace WebApp.Shared.Validations.Customer.Order
{
    public class CreateReturnRequestDtoValidator : AbstractValidator<CreateReturnRequestDto>
    {
        private const int MaxBase64ImageLength = 5 * 1024 * 1024; // 5MB file nhị phân
        private const int MaxEvidenceImageCount = 5;

        public CreateReturnRequestDtoValidator()
        {
            RuleFor(x => x.OrderId)
                .GreaterThan(0).WithMessage("Mã đơn hàng không hợp lệ");

            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("Vui lòng nhập lý do yêu cầu trả hàng")
                .MaximumLength(1000).WithMessage("Lý do trả hàng tối đa 1000 ký tự");

            When(x => !string.IsNullOrEmpty(x.DetailDescription), () =>
            {
                RuleFor(x => x.DetailDescription!)
                    .MaximumLength(2000).WithMessage("Mô tả chi tiết tối đa 2000 ký tự");
            });

            When(x => !string.IsNullOrEmpty(x.BankAccountNumber), () =>
            {
                RuleFor(x => x.BankAccountNumber!)
                    .MaximumLength(50).WithMessage("Số tài khoản ngân hàng tối đa 50 ký tự");
            });

            When(x => x.Items != null && x.Items.Count > 0, () =>
            {
                RuleForEach(x => x.Items!).ChildRules(item =>
                {
                    item.RuleFor(i => i.ProductId)
                        .GreaterThan(0).WithMessage("ProductId không hợp lệ");
                    item.RuleFor(i => i.Quantity)
                        .GreaterThan(0).WithMessage("Số lượng hoàn trả phải lớn hơn 0");
                });
            });

            // Kiểm tra danh sách ảnh minh chứng (Base64)
            When(x => x.EvidenceImagesBase64 != null && x.EvidenceImagesBase64.Count > 0, () =>
            {
                RuleFor(x => x.EvidenceImagesBase64!.Count)
                    .LessThanOrEqualTo(MaxEvidenceImageCount)
                    .WithMessage($"Số lượng ảnh minh chứng không được vượt quá {MaxEvidenceImageCount} ảnh");

                RuleForEach(x => x.EvidenceImagesBase64!)
                    .Must(base64 => !string.IsNullOrWhiteSpace(base64) && base64.Length <= MaxBase64ImageLength)
                    .WithMessage("Mỗi ảnh minh chứng không được vượt quá 5MB")
                    .Must(ImageSignatureHelper.IsValidImageBase64)
                    .WithMessage("Ảnh minh chứng không hợp lệ hoặc không đúng định dạng (chỉ chấp nhận PNG, JPG, JPEG, WEBP)");
            });
        }
    }
}
