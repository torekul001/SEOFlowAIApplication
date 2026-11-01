using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class DraftApproval
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid DraftVersionId { get; set; }

    public Guid ApproverId { get; set; }

    public string Decision { get; set; } = null!;

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual User Approver { get; set; } = null!;

    public virtual DraftVersion DraftVersion { get; set; } = null!;

    public virtual ICollection<PublishJob> PublishJobs { get; set; } = new List<PublishJob>();

    public virtual Tenant Tenant { get; set; } = null!;
}
