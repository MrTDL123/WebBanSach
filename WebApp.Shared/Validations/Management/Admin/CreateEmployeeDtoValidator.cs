using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.Shared.Dtos.AdminClient.SystemAdmin;

namespace WebApp.Shared.Validations.Admin.SystemAdmin
{
    // CreateEmployeeDtoValidator.cs
    public class CreateEmployeeDtoValidator : AbstractValidator<CreateEmployeeDto>
    {
        private static readonly string[] ValidRoles = {
        "ContentAndMarketingManager", "OrderAndLogisticsManager", "CustomerSupport",
        "WarehouseAndInventoryManager", "FinanceAndAccountant"
    };
        public CreateEmployeeDtoValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.PhoneNumber).NotEmpty().Matches(@"^(0\d{9})$");
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Vui lòng chọn vai trò nhân viên")
                .Must(r => ValidRoles.Contains(r))
                .WithMessage("Vai trò không hợp lệ");
        }
    }
}
