using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using WebApp.Shared.Dtos.Management.Admin;

namespace WebApp.Shared.Validations.Management.Admin
{
    public class BanUserDtoValidator : AbstractValidator<BanUserDto>
    {
        public BanUserDtoValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId không được để trống.");
            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("Lý do cấm không được để trống.")
                .MaximumLength(500).WithMessage("Lý do cấm không được vượt quá 500 ký tự.");
        }
    }
}
