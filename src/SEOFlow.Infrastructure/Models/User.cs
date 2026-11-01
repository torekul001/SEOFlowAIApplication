using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string NormalizedEmail { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public bool EmailConfirmed { get; set; }

    public int FailedAttempts { get; set; }

    public DateTime? LockoutEndUtc { get; set; }

    public Guid SecurityStamp { get; set; }

    public DateTime CreatedUtc { get; set; }

    public bool IsActive { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual ICollection<AccountToken> AccountTokens { get; set; } = new List<AccountToken>();

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<ContentDraft> ContentDrafts { get; set; } = new List<ContentDraft>();

    public virtual ICollection<DraftApproval> DraftApprovals { get; set; } = new List<DraftApproval>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<TenantMembership> TenantMemberships { get; set; } = new List<TenantMembership>();
}
