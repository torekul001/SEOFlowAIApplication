using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class KeywordGroup
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid WebsiteId { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual ICollection<Keyword> Keywords { get; set; } = new List<Keyword>();

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual Website Website { get; set; } = null!;
}
