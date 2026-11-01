using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class AuditLog
{
    public long Id { get; set; }

    public Guid? TenantId { get; set; }

    public Guid? UserId { get; set; }

    public string Action { get; set; } = null!;

    public string Detail { get; set; } = null!;

    public DateTime CreatedUtc { get; set; }

    public virtual Tenant? Tenant { get; set; }

    public virtual User? User { get; set; }
}
