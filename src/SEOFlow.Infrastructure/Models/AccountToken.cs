using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class AccountToken
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Purpose { get; set; } = null!;

    public string TokenHash { get; set; } = null!;

    public DateTime ExpiresUtc { get; set; }

    public DateTime? UsedUtc { get; set; }

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
