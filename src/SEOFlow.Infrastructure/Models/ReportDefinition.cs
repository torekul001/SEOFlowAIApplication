using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class ReportDefinition
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public string Name { get; set; } = null!;

    public string SettingsJson { get; set; } = null!;

    public bool EmailOptIn { get; set; }

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual ICollection<ReportRun> ReportRuns { get; set; } = new List<ReportRun>();

    public virtual Tenant Tenant { get; set; } = null!;
}
