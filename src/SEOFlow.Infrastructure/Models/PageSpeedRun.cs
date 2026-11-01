using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class PageSpeedRun
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid WebsiteId { get; set; }

    public string Url { get; set; } = null!;

    public string Device { get; set; } = null!;

    public string? LabMetricsJson { get; set; }

    public string? FieldMetricsJson { get; set; }

    public string? ProviderError { get; set; }

    public DateTime TestedUtc { get; set; }

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual Website Website { get; set; } = null!;
}
