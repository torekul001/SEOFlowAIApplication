using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class AiJob
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid WebsiteId { get; set; }

    public Guid? PromptId { get; set; }

    public string Status { get; set; } = null!;

    public string? Model { get; set; }

    public string? Error { get; set; }

    public DateTime? FinishedUtc { get; set; }

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual AiPrompt? AiPrompt { get; set; }

    public virtual ICollection<AiUsage> AiUsages { get; set; } = new List<AiUsage>();

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual Website Website { get; set; } = null!;
}
