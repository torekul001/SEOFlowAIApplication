using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class TenantMembership
{
    public Guid TenantId { get; set; }

    public Guid UserId { get; set; }

    public string Role { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual Role RoleNavigation { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
