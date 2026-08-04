using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanSach.Shared.Dtos.AdminClient.ContentAndMarketing;

namespace WebBanSach.Api.Validations.Admin.ContentAndMarketing
{
    // CreateAuthorDtoValidator.cs
    public class CreateAuthorDtoValidator : AbstractValidator<CreateAuthorDto>
    {
        public CreateAuthorDtoValidator()
        {
            RuleFor(x => x.AuthorName)
                .NotEmpty().WithMessage("Tên tác giả không được để trống")
                .MaximumLength(150).WithMessage("Tên tác giả tối đa 150 ký tự");
        }
    }
}
