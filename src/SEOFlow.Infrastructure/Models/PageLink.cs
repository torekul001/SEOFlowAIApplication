using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class PageLink
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid WebsiteId { get; set; }

    public Guid PageId { get; set; }

    public string TargetUrl { get; set; } = null!;

    public bool IsInternal { get; set; }

    public bool IsNoFollow { get; set; }

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual CrawledPage CrawledPage { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual Website Website { get; set; } = null!;
}
