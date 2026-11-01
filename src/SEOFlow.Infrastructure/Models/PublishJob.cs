using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class PublishJob
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid WebsiteId { get; set; }

    public Guid DraftVersionId { get; set; }

    public Guid ApprovalId { get; set; }

    public string Status { get; set; } = null!;

    public Guid IdempotencyKey { get; set; }

    public bool IsDryRun { get; set; }

    public string? Error { get; set; }

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual DraftApproval DraftApproval { get; set; } = null!;

    public virtual DraftVersion DraftVersion { get; set; } = null!;

    public virtual ICollection<PublishBackup> PublishBackups { get; set; } = new List<PublishBackup>();

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual Website Website { get; set; } = null!;
}
