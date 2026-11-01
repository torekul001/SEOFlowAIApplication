using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class Invitation
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public string Email { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string TokenHash { get; set; } = null!;

    public DateTime ExpiresUtc { get; set; }

    public DateTime? AcceptedUtc { get; set; }

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual Role RoleNavigation { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;
}
