using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class ContentDraft
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid WebsiteId { get; set; }

    public string PageUrl { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string Locale { get; set; } = null!;

    public Guid AuthorId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual User Author { get; set; } = null!;

    public virtual ICollection<DraftVersion> DraftVersions { get; set; } = new List<DraftVersion>();

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual Website Website { get; set; } = null!;
}
