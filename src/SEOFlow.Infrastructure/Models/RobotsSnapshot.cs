using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class RobotsSnapshot
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid WebsiteId { get; set; }

    public string Content { get; set; } = null!;

    public int HttpStatus { get; set; }

    public DateTime FetchedUtc { get; set; }

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual Website Website { get; set; } = null!;
}
