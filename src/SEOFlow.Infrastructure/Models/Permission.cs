using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class Permission
{
    public string Name { get; set; } = null!;

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
