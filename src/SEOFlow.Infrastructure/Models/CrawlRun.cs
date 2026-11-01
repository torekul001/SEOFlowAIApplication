using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class CrawlRun
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid WebsiteId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? StartedUtc { get; set; }

    public DateTime? FinishedUtc { get; set; }

    public int MaxPages { get; set; }

    public int PagesProcessed { get; set; }

    public string? Error { get; set; }

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual ICollection<CrawledPage> CrawledPages { get; set; } = new List<CrawledPage>();

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual Website Website { get; set; } = null!;
}
