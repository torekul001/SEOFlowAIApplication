using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class BacklinkSnapshot
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid WebsiteId { get; set; }

    public string Provider { get; set; } = null!;

    public string SourceUrl { get; set; } = null!;

    public string TargetUrl { get; set; } = null!;

    public DateTime ObservedUtc { get; set; }

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual Website Website { get; set; } = null!;
}
