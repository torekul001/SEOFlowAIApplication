using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class FindingHistory
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid WebsiteId { get; set; }

    public Guid FindingId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual SeoFinding SeoFinding { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual Website Website { get; set; } = null!;
}
