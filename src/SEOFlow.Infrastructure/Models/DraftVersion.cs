using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class DraftVersion
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid DraftId { get; set; }

    public int Version { get; set; }

    public string BeforeJson { get; set; } = null!;

    public string AfterJson { get; set; } = null!;

    public string EvidenceJson { get; set; } = null!;

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual ContentDraft ContentDraft { get; set; } = null!;

    public virtual ICollection<DraftApproval> DraftApprovals { get; set; } = new List<DraftApproval>();

    public virtual ICollection<PublishJob> PublishJobs { get; set; } = new List<PublishJob>();

    public virtual Tenant Tenant { get; set; } = null!;
}
