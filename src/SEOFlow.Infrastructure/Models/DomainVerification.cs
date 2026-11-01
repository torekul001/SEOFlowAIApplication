using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class DomainVerification
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid WebsiteId { get; set; }

    public string Method { get; set; } = null!;

    public string ChallengeHash { get; set; } = null!;

    public DateTime? VerifiedUtc { get; set; }

    public DateTime ExpiresUtc { get; set; }

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual Website Website { get; set; } = null!;
}
