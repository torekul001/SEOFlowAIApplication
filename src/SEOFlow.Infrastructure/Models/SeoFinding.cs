using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class SeoFinding
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid WebsiteId { get; set; }

    public Guid PageId { get; set; }

    public string RuleCode { get; set; } = null!;

    public string Evidence { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual CrawledPage CrawledPage { get; set; } = null!;

    public virtual ICollection<FindingHistory> FindingHistories { get; set; } = new List<FindingHistory>();

    public virtual SeoRule RuleCodeNavigation { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual Website Website { get; set; } = null!;
}
