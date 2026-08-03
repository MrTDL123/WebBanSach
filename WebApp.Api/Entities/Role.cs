using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace WebApp.Api.Entities;

public partial class Role : IdentityRole
{
    public string? Description { get; set; }
}
