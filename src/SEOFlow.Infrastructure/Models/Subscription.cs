using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class Subscription
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid PlanId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? EndsUtc { get; set; }

    public string? BillingReference { get; set; }

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual Plan Plan { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;
}
