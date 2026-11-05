using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using WebApp.Shared.Dtos.Management.Admin;

namespace WebApp.Shared.Validations.Management.Admin
{
    public class UpdateEmployeeDtoValidator : AbstractValidator<UpdateEmployeeDto>
    {
        private static readonly string[] ValidRoles = {
            "ContentManager", "OrderManager", "CustomerSupport",
            "WarehouseManager", "Accountant"
        };

        public UpdateEmployeeDtoValidator()
        {
            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Vui lòng chọn vai trò nhân viên")
                .Must(r => ValidRoles.Contains(r))
                .WithMessage("Vai trò không hợp lệ");
        }
    }
}
