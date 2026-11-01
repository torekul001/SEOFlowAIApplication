using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class ScheduledJob
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid? WebsiteId { get; set; }

    public string Kind { get; set; } = null!;

    public string CronExpression { get; set; } = null!;

    public bool IsEnabled { get; set; }

    public bool IsDryRun { get; set; }

    public DateTime? NextRunUtc { get; set; }

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual Website? Website { get; set; }
}
