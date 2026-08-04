using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanSach.Shared.Dtos.CustomerDtos.FreeBook;

namespace WebBanSach.Api.Validations.Customer.FreeBook
{

    // CreateBookDonationDtoValidator.cs
    public class CreateBookDonationDtoValidator : AbstractValidator<CreateBookDonationDto>
    {
        public CreateBookDonationDtoValidator()
        {
            RuleFor(x => x.DonorName).NotEmpty().WithMessage("Vui lòng nhập tên người tặng");
            RuleFor(x => x.DonorPhone)
                .NotEmpty().WithMessage("Số điện thoại không được để trống")
                .Matches(@"^(0\d{9})$").WithMessage("Số điện thoại không hợp lệ");
            RuleFor(x => x.DonorEmail)
                .EmailAddress().WithMessage("Email không hợp lệ")
                .When(x => !string.IsNullOrEmpty(x.DonorEmail));
            RuleFor(x => x.PickupAddress)
                .NotEmpty().WithMessage("Vui lòng nhập địa chỉ để nhân viên đến lấy sách");
            RuleFor(x => x.Books)
                .NotEmpty().WithMessage("Vui lòng thêm ít nhất 1 cuốn sách muốn tặng");
            RuleForEach(x => x.Books).ChildRules(book =>
            {
                book.RuleFor(b => b.BookTitle).NotEmpty().WithMessage("Tên sách không được để trống");
                book.RuleFor(b => b.Quantity).GreaterThan(0);
            });
        }
    }
}
