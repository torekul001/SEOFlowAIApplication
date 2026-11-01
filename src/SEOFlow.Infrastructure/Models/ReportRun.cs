using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class ReportRun
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid DefinitionId { get; set; }

    public string Status { get; set; } = null!;

    public string? StorageKey { get; set; }

    public DateTime? FinishedUtc { get; set; }

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual ReportDefinition ReportDefinition { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;
}
