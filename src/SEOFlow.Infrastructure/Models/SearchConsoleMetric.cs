using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class SearchConsoleMetric
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid WebsiteId { get; set; }

    public DateOnly MetricDate { get; set; }

    public string Query { get; set; } = null!;

    public string PageUrl { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string Device { get; set; } = null!;

    public long Clicks { get; set; }

    public long Impressions { get; set; }

    public decimal Ctr { get; set; }

    public decimal AveragePosition { get; set; }

    public DateTime SyncedUtc { get; set; }

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual Website Website { get; set; } = null!;
}
