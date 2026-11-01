using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class Sitemap
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid WebsiteId { get; set; }

    public string Url { get; set; } = null!;

    public DateTime? FetchedUtc { get; set; }

    public int? HttpStatus { get; set; }

    public byte[]? ContentHash { get; set; }

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual Website Website { get; set; } = null!;
}
