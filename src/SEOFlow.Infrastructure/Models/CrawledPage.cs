using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class CrawledPage
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid WebsiteId { get; set; }

    public Guid CrawlRunId { get; set; }

    public string Url { get; set; } = null!;

    public byte[] UrlHash { get; set; } = null!;

    public string? FinalUrl { get; set; }

    public int? HttpStatus { get; set; }

    public string? Title { get; set; }

    public string? MetaDescription { get; set; }

    public string? CanonicalUrl { get; set; }

    public byte[]? ContentHash { get; set; }

    public int? DurationMs { get; set; }

    public DateTime CrawledUtc { get; set; }

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual CrawlRun CrawlRun { get; set; } = null!;

    public virtual ICollection<PageLink> PageLinks { get; set; } = new List<PageLink>();

    public virtual ICollection<SeoFinding> SeoFindings { get; set; } = new List<SeoFinding>();

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual Website Website { get; set; } = null!;
}
