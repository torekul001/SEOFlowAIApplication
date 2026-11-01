using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class AnalyticsMetric
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid WebsiteId { get; set; }

    public DateOnly MetricDate { get; set; }

    public string PagePath { get; set; } = null!;

    public long Sessions { get; set; }

    public decimal Conversions { get; set; }

    public DateTime SyncedUtc { get; set; }

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual Website Website { get; set; } = null!;
}
