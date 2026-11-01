using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class KeywordRankSnapshot
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid WebsiteId { get; set; }

    public Guid KeywordId { get; set; }

    public string Provider { get; set; } = null!;

    public int? Position { get; set; }

    public string Device { get; set; } = null!;

    public string Location { get; set; } = null!;

    public DateTime CheckedUtc { get; set; }

    public decimal? Cost { get; set; }

    public string? Error { get; set; }

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual Keyword Keyword { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual Website Website { get; set; } = null!;
}
