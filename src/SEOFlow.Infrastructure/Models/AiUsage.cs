using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class AiUsage
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid AiJobId { get; set; }

    public int InputTokens { get; set; }

    public int OutputTokens { get; set; }

    public decimal Cost { get; set; }

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual AiJob AiJob { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;
}
