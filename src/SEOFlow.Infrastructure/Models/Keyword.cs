using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class Keyword
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid WebsiteId { get; set; }

    public Guid? GroupId { get; set; }

    public string Phrase { get; set; } = null!;

    public string? TargetUrl { get; set; }

    public string? Intent { get; set; }

    public string Locale { get; set; } = null!;

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual KeywordGroup? KeywordGroup { get; set; }

    public virtual ICollection<KeywordRankSnapshot> KeywordRankSnapshots { get; set; } = new List<KeywordRankSnapshot>();

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual Website Website { get; set; } = null!;
}
