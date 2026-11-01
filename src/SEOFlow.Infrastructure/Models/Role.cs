using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class Role
{
    public string Name { get; set; } = null!;

    public virtual ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();

    public virtual ICollection<TenantMembership> TenantMemberships { get; set; } = new List<TenantMembership>();

    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
