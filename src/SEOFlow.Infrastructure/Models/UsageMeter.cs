using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class UsageMeter
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public string Metric { get; set; } = null!;

    public DateOnly PeriodStart { get; set; }

    public long Quantity { get; set; }

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;
}
