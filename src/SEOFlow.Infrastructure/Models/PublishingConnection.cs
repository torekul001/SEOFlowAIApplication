using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class PublishingConnection
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid WebsiteId { get; set; }

    public string Provider { get; set; } = null!;

    public string Endpoint { get; set; } = null!;

    public byte[]? EncryptedCredentials { get; set; }

    public bool IsEnabled { get; set; }

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual Website Website { get; set; } = null!;
}
