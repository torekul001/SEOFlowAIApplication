using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class ProviderConnection
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid WebsiteId { get; set; }

    public string Provider { get; set; } = null!;

    public byte[]? EncryptedTokens { get; set; }

    public string? Scopes { get; set; }

    public DateTime? ExpiresUtc { get; set; }

    public DateTime? DisconnectedUtc { get; set; }

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual ICollection<ProviderSyncRun> ProviderSyncRuns { get; set; } = new List<ProviderSyncRun>();

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual Website Website { get; set; } = null!;
}
