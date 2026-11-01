using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class KeywordSuggestion
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid WebsiteId { get; set; }

    public string Phrase { get; set; } = null!;

    public string Source { get; set; } = null!;

    public bool IsAiSuggestion { get; set; }

    public long? SearchVolume { get; set; }

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual Website Website { get; set; } = null!;
}
