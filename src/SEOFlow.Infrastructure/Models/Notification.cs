using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class Notification
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid UserId { get; set; }

    public string Kind { get; set; } = null!;

    public string Message { get; set; } = null!;

    public DateTime? ReadUtc { get; set; }

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
