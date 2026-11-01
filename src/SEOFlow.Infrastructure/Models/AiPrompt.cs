using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class AiPrompt
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public string Name { get; set; } = null!;

    public int Version { get; set; }

    public string Template { get; set; } = null!;

    public string Locale { get; set; } = null!;

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual ICollection<AiJob> AiJobs { get; set; } = new List<AiJob>();

    public virtual Tenant Tenant { get; set; } = null!;
}
