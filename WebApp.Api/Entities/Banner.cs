using System;
using System.Collections.Generic;

namespace WebApp.Api.Entities;

public partial class Banner
{
    public int BannerId { get; set; }

    public string Title { get; set; } = null!;

    public string ImageUrl { get; set; } = null!;

    public string? TargetUrl { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }
}
