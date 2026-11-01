using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class PublishBackup
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid PublishJobId { get; set; }

    public string OriginalJson { get; set; } = null!;

    public byte[] OriginalContentHash { get; set; } = null!;

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual PublishJob PublishJob { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;
}
