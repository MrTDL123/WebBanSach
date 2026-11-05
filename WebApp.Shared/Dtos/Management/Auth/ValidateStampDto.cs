using System;
using System.Collections.Generic;
using System.Text;

namespace WebApp.Shared.Dtos.Management.Auth
{
    public record ValidateStampDto(
        string SecurityStamp,
        List<string> CurrentRoles);
}
