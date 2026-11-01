using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SEOFlow.Infrastructure.Models;

namespace SEOFlow.Infrastructure.Data;

public partial class SeoFlowDbContext : DbContext
{
    public SeoFlowDbContext(DbContextOptions<SeoFlowDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccountToken> AccountTokens { get; set; }

    public virtual DbSet<AiJob> AiJobs { get; set; }

    public virtual DbSet<AiPrompt> AiPrompts { get; set; }

    public virtual DbSet<AiUsage> AiUsages { get; set; }

    public virtual DbSet<AnalyticsMetric> AnalyticsMetrics { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<BacklinkSnapshot> BacklinkSnapshots { get; set; }

    public virtual DbSet<Competitor> Competitors { get; set; }

    public virtual DbSet<ContentDraft> ContentDrafts { get; set; }

    public virtual DbSet<CrawlRun> CrawlRuns { get; set; }

    public virtual DbSet<CrawledPage> CrawledPages { get; set; }

    public virtual DbSet<DomainVerification> DomainVerifications { get; set; }

    public virtual DbSet<DraftApproval> DraftApprovals { get; set; }

    public virtual DbSet<DraftVersion> DraftVersions { get; set; }

    public virtual DbSet<FindingHistory> FindingHistories { get; set; }

    public virtual DbSet<Invitation> Invitations { get; set; }

    public virtual DbSet<Keyword> Keywords { get; set; }

    public virtual DbSet<KeywordGroup> KeywordGroups { get; set; }

    public virtual DbSet<KeywordRankSnapshot> KeywordRankSnapshots { get; set; }

    public virtual DbSet<KeywordSuggestion> KeywordSuggestions { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<PageLink> PageLinks { get; set; }

    public virtual DbSet<PageSpeedRun> PageSpeedRuns { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Plan> Plans { get; set; }

    public virtual DbSet<ProviderConnection> ProviderConnections { get; set; }

    public virtual DbSet<ProviderSyncRun> ProviderSyncRuns { get; set; }

    public virtual DbSet<PublishBackup> PublishBackups { get; set; }

    public virtual DbSet<PublishJob> PublishJobs { get; set; }

    public virtual DbSet<PublishingConnection> PublishingConnections { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<ReportDefinition> ReportDefinitions { get; set; }

    public virtual DbSet<ReportRun> ReportRuns { get; set; }

    public virtual DbSet<RobotsSnapshot> RobotsSnapshots { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<ScheduledJob> ScheduledJobs { get; set; }

    public virtual DbSet<SchemaVersion> SchemaVersions { get; set; }

    public virtual DbSet<SearchConsoleMetric> SearchConsoleMetrics { get; set; }

    public virtual DbSet<SeoFinding> SeoFindings { get; set; }

    public virtual DbSet<SeoRule> SeoRules { get; set; }

    public virtual DbSet<Sitemap> Sitemaps { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<Tenant> Tenants { get; set; }

    public virtual DbSet<TenantMembership> TenantMemberships { get; set; }

    public virtual DbSet<UsageMeter> UsageMeters { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Website> Websites { get; set; }

    public virtual DbSet<WebsiteSetting> WebsiteSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AccountT__3214EC07F105733A");

            entity.HasIndex(e => e.TokenHash, "UQ__AccountT__BCB33F928D2F1694").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Purpose)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.TokenHash)
                .HasMaxLength(64)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.AccountTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AccountTo__UserI__5BE2A6F2");
        });

        modelBuilder.Entity<AiJob>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AiJobs__3214EC074891BF5F");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_AiJobs_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_AiJobs_TenantId_Id").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Error).HasMaxLength(2000);
            entity.Property(e => e.Model).HasMaxLength(100);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Tenant).WithMany(p => p.AiJobs)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AiJobs__TenantId__27F8EE98");

            entity.HasOne(d => d.AiPrompt).WithMany(p => p.AiJobs)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.PromptId })
                .HasConstraintName("FK__AiJobs__28ED12D1");

            entity.HasOne(d => d.Website).WithMany(p => p.AiJobs)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.WebsiteId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AiJobs_Websites");
        });

        modelBuilder.Entity<AiPrompt>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AiPrompt__3214EC0783DEC4D8");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_AiPrompts_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_AiPrompts_TenantId_Id").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Locale)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Tenant).WithMany(p => p.AiPrompts)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AiPrompts__Tenan__214BF109");
        });

        modelBuilder.Entity<AiUsage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AiUsage__3214EC07060C3446");

            entity.ToTable("AiUsage");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_AiUsage_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_AiUsage_TenantId_Id").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Cost).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Tenant).WithMany(p => p.AiUsages)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AiUsage__TenantI__2F9A1060");

            entity.HasOne(d => d.AiJob).WithMany(p => p.AiUsages)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.AiJobId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AiUsage__336AA144");
        });

        modelBuilder.Entity<AnalyticsMetric>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Analytic__3214EC07FAAF633E");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_AnalyticsMetrics_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_AnalyticsMetrics_TenantId_Id").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Conversions).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.PagePath).HasMaxLength(2048);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Tenant).WithMany(p => p.AnalyticsMetrics)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Analytics__Tenan__6BE40491");

            entity.HasOne(d => d.Website).WithMany(p => p.AnalyticsMetrics)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.WebsiteId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AnalyticsMetrics_Websites");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AuditLog__3214EC077012EAA6");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_AuditLogs_Tenant_Time").IsDescending(false, true);

            entity.Property(e => e.Action).HasMaxLength(100);
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Detail).HasMaxLength(2000);

            entity.HasOne(d => d.Tenant).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.TenantId)
                .HasConstraintName("FK__AuditLogs__Tenan__60A75C0F");

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__AuditLogs__UserI__619B8048");
        });

        modelBuilder.Entity<BacklinkSnapshot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Backlink__3214EC0746D76C2C");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_BacklinkSnapshots_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_BacklinkSnapshots_TenantId_Id").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Provider)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.SourceUrl).HasMaxLength(2048);
            entity.Property(e => e.TargetUrl).HasMaxLength(2048);

            entity.HasOne(d => d.Tenant).WithMany(p => p.BacklinkSnapshots)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BacklinkS__Tenan__1A9EF37A");

            entity.HasOne(d => d.Website).WithMany(p => p.BacklinkSnapshots)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.WebsiteId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BacklinkSnapshots_Websites");
        });

        modelBuilder.Entity<Competitor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Competit__3214EC0732BD3D3C");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_Competitors_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_Competitors_TenantId_Id").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Domain).HasMaxLength(253);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Tenant).WithMany(p => p.Competitors)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Competito__Tenan__13F1F5EB");

            entity.HasOne(d => d.Website).WithMany(p => p.Competitors)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.WebsiteId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Competitors_Websites");
        });

        modelBuilder.Entity<ContentDraft>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ContentD__3214EC077A0E4C45");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_ContentDrafts_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_ContentDrafts_TenantId_Id").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Locale)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PageUrl).HasMaxLength(2048);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Author).WithMany(p => p.ContentDrafts)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ContentDr__Autho__3B0BC30C");

            entity.HasOne(d => d.Tenant).WithMany(p => p.ContentDrafts)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ContentDr__Tenan__39237A9A");

            entity.HasOne(d => d.Website).WithMany(p => p.ContentDrafts)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.WebsiteId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ContentDrafts_Websites");
        });

        modelBuilder.Entity<CrawlRun>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CrawlRun__3214EC077E49E4E6");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_CrawlRuns_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_CrawlRuns_TenantId_Id").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Error).HasMaxLength(2000);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Tenant).WithMany(p => p.CrawlRuns)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CrawlRuns__Tenan__0D7A0286");

            entity.HasOne(d => d.Website).WithMany(p => p.CrawlRuns)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.WebsiteId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CrawlRuns_Websites");
        });

        modelBuilder.Entity<CrawledPage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CrawledP__3214EC074AFC3BF6");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_CrawledPages_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_CrawledPages_TenantId_Id").IsUnique();

            entity.HasIndex(e => new { e.TenantId, e.CrawlRunId, e.UrlHash }, "UX_CrawledPages_Run_Url").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CanonicalUrl).HasMaxLength(2048);
            entity.Property(e => e.ContentHash)
                .HasMaxLength(32)
                .IsFixedLength();
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.FinalUrl).HasMaxLength(2048);
            entity.Property(e => e.MetaDescription).HasMaxLength(2000);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Title).HasMaxLength(1000);
            entity.Property(e => e.Url).HasMaxLength(2048);
            entity.Property(e => e.UrlHash)
                .HasMaxLength(32)
                .IsFixedLength();

            entity.HasOne(d => d.Tenant).WithMany(p => p.CrawledPages)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CrawledPa__Tenan__17036CC0");

            entity.HasOne(d => d.CrawlRun).WithMany(p => p.CrawledPages)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.CrawlRunId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CrawledPages__19DFD96B");

            entity.HasOne(d => d.Website).WithMany(p => p.CrawledPages)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.WebsiteId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CrawledPages_Websites");
        });

        modelBuilder.Entity<DomainVerification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DomainVe__3214EC07B0C8DAA6");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_DomainVerifications_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_DomainVerifications_TenantId_Id").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ChallengeHash)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Method)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Tenant).WithMany(p => p.DomainVerifications)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DomainVer__Tenan__74AE54BC");

            entity.HasOne(d => d.Website).WithMany(p => p.DomainVerifications)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.WebsiteId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DomainVerifications_Websites");
        });

        modelBuilder.Entity<DraftApproval>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DraftApp__3214EC07182D9C34");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_DraftApprovals_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_DraftApprovals_TenantId_Id").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Decision)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Approver).WithMany(p => p.DraftApprovals)
                .HasForeignKey(d => d.ApproverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DraftAppr__Appro__4E1E9780");

            entity.HasOne(d => d.Tenant).WithMany(p => p.DraftApprovals)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DraftAppr__Tenan__4D2A7347");

            entity.HasOne(d => d.DraftVersion).WithMany(p => p.DraftApprovals)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.DraftVersionId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DraftApprovals__5006DFF2");
        });

        modelBuilder.Entity<DraftVersion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DraftVer__3214EC07956F74F5");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_DraftVersions_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_DraftVersions_TenantId_Id").IsUnique();

            entity.HasIndex(e => new { e.TenantId, e.DraftId, e.Version }, "UX_DraftVersions_Number").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Tenant).WithMany(p => p.DraftVersions)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DraftVers__Tenan__42ACE4D4");

            entity.HasOne(d => d.ContentDraft).WithMany(p => p.DraftVersions)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.DraftId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DraftVersions__477199F1");
        });

        modelBuilder.Entity<FindingHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FindingH__3214EC07277559FB");

            entity.ToTable("FindingHistory");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_FindingHistory_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_FindingHistory_TenantId_Id").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Tenant).WithMany(p => p.FindingHistories)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FindingHi__Tenan__367C1819");

            entity.HasOne(d => d.SeoFinding).WithMany(p => p.FindingHistories)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.FindingId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FindingHistory__37703C52");

            entity.HasOne(d => d.Website).WithMany(p => p.FindingHistories)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.WebsiteId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FindingHistory_Websites");
        });

        modelBuilder.Entity<Invitation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Invitati__3214EC0725A795EF");

            entity.HasIndex(e => e.TokenHash, "UQ__Invitati__BCB33F92DD028D73").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.Role)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.TokenHash)
                .HasMaxLength(64)
                .IsUnicode(false);

            entity.HasOne(d => d.RoleNavigation).WithMany(p => p.Invitations)
                .HasForeignKey(d => d.Role)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Invitation__Role__5629CD9C");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Invitations)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Invitatio__Tenan__5535A963");
        });

        modelBuilder.Entity<Keyword>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Keywords__3214EC07878BCED4");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_Keywords_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_Keywords_TenantId_Id").IsUnique();

            entity.HasIndex(e => new { e.TenantId, e.WebsiteId, e.Phrase, e.Locale }, "UX_Keywords_Phrase").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Intent)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Locale)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.TargetUrl).HasMaxLength(2048);

            entity.HasOne(d => d.Tenant).WithMany(p => p.Keywords)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Keywords__Tenant__7B264821");

            entity.HasOne(d => d.KeywordGroup).WithMany(p => p.Keywords)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.GroupId })
                .HasConstraintName("FK__Keywords__7C1A6C5A");

            entity.HasOne(d => d.Website).WithMany(p => p.Keywords)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.WebsiteId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Keywords_Websites");
        });

        modelBuilder.Entity<KeywordGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__KeywordG__3214EC07623F5572");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_KeywordGroups_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_KeywordGroups_TenantId_Id").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Tenant).WithMany(p => p.KeywordGroups)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KeywordGr__Tenan__74794A92");

            entity.HasOne(d => d.Website).WithMany(p => p.KeywordGroups)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.WebsiteId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KeywordGroups_Websites");
        });

        modelBuilder.Entity<KeywordRankSnapshot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__KeywordR__3214EC07F4D33A8D");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_KeywordRankSnapshots_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_KeywordRankSnapshots_TenantId_Id").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Cost).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Device)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Error).HasMaxLength(2000);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.Provider)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Tenant).WithMany(p => p.KeywordRankSnapshots)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KeywordRa__Tenan__0A688BB1");

            entity.HasOne(d => d.Keyword).WithMany(p => p.KeywordRankSnapshots)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.KeywordId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KeywordRankSnaps__0D44F85C");

            entity.HasOne(d => d.Website).WithMany(p => p.KeywordRankSnapshots)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.WebsiteId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KeywordRankSnapshots_Websites");
        });

        modelBuilder.Entity<KeywordSuggestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__KeywordS__3214EC0744386C7D");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_KeywordSuggestions_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_KeywordSuggestions_TenantId_Id").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Phrase).HasMaxLength(450);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Source)
                .HasMaxLength(40)
                .IsUnicode(false);

            entity.HasOne(d => d.Tenant).WithMany(p => p.KeywordSuggestions)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KeywordSu__Tenan__02C769E9");

            entity.HasOne(d => d.Website).WithMany(p => p.KeywordSuggestions)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.WebsiteId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KeywordSuggestions_Websites");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3214EC07BA24B648");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_Notifications_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_Notifications_TenantId_Id").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Kind)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.Message).HasMaxLength(2000);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Tenant).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__Tenan__7073AF84");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__UserI__7167D3BD");
        });

        modelBuilder.Entity<PageLink>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PageLink__3214EC0711C258BB");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_PageLinks_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_PageLinks_TenantId_Id").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.TargetUrl).HasMaxLength(2048);

            entity.HasOne(d => d.Tenant).WithMany(p => p.PageLinks)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PageLinks__Tenan__208CD6FA");

            entity.HasOne(d => d.CrawledPage).WithMany(p => p.PageLinks)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.PageId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PageLinks__2180FB33");

            entity.HasOne(d => d.Website).WithMany(p => p.PageLinks)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.WebsiteId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PageLinks_Websites");
        });

        modelBuilder.Entity<PageSpeedRun>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PageSpee__3214EC07FD7F60F1");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_PageSpeedRuns_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_PageSpeedRuns_TenantId_Id").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Device)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.ProviderError).HasMaxLength(2000);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Url).HasMaxLength(2048);

            entity.HasOne(d => d.Tenant).WithMany(p => p.PageSpeedRuns)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PageSpeed__Tenan__4B7734FF");

            entity.HasOne(d => d.Website).WithMany(p => p.PageSpeedRuns)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.WebsiteId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PageSpeedRuns_Websites");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Name).HasName("PK__Permissi__737584F7C2F1C5C7");

            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Plan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Plans__3214EC07E2EE1A22");

            entity.HasIndex(e => e.Name, "UQ__Plans__737584F6ECE3D7BE").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<ProviderConnection>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Provider__3214EC07073E525A");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_ProviderConnections_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_ProviderConnections_TenantId_Id").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Provider)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Scopes).HasMaxLength(2000);

            entity.HasOne(d => d.Tenant).WithMany(p => p.ProviderConnections)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProviderC__Tenan__55009F39");

            entity.HasOne(d => d.Website).WithMany(p => p.ProviderConnections)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.WebsiteId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProviderConnections_Websites");
        });

        modelBuilder.Entity<ProviderSyncRun>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Provider__3214EC07B3F9151A");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_ProviderSyncRuns_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_ProviderSyncRuns_TenantId_Id").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Error).HasMaxLength(2000);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Tenant).WithMany(p => p.ProviderSyncRuns)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProviderS__Tenan__5BAD9CC8");

            entity.HasOne(d => d.ProviderConnection).WithMany(p => p.ProviderSyncRuns)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.ConnectionId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProviderSyncRuns__5CA1C101");

            entity.HasOne(d => d.Website).WithMany(p => p.ProviderSyncRuns)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.WebsiteId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProviderSyncRuns_Websites");
        });

        modelBuilder.Entity<PublishBackup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PublishB__3214EC07E2D4AC46");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_PublishBackups_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_PublishBackups_TenantId_Id").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.OriginalContentHash)
                .HasMaxLength(32)
                .IsFixedLength();
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Tenant).WithMany(p => p.PublishBackups)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PublishBa__Tenan__603D47BB");

            entity.HasOne(d => d.PublishJob).WithMany(p => p.PublishBackups)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.PublishJobId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PublishBackups__6225902D");
        });

        modelBuilder.Entity<PublishJob>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PublishJ__3214EC0711B3D1C9");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_PublishJobs_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_PublishJobs_TenantId_Id").IsUnique();

            entity.HasIndex(e => e.IdempotencyKey, "UQ__PublishJ__A6D161D84E4B39E3").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Error).HasMaxLength(2000);
            entity.Property(e => e.IsDryRun).HasDefaultValue(true);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Tenant).WithMany(p => p.PublishJobs)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PublishJo__Tenan__56B3DD81");

            entity.HasOne(d => d.DraftApproval).WithMany(p => p.PublishJobs)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.ApprovalId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PublishJobs__59904A2C");

            entity.HasOne(d => d.DraftVersion).WithMany(p => p.PublishJobs)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.DraftVersionId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PublishJobs__589C25F3");

            entity.HasOne(d => d.Website).WithMany(p => p.PublishJobs)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.WebsiteId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PublishJobs_Websites");
        });

        modelBuilder.Entity<PublishingConnection>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Publishi__3214EC0715721A76");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_PublishingConnections_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_PublishingConnections_TenantId_Id").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Endpoint).HasMaxLength(2048);
            entity.Property(e => e.Provider)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Tenant).WithMany(p => p.PublishingConnections)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Publishin__Tenan__05D8E0BE");

            entity.HasOne(d => d.Website).WithMany(p => p.PublishingConnections)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.WebsiteId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PublishingConnections_Websites");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RefreshT__3214EC0707568DEC");

            entity.HasIndex(e => e.FamilyId, "IX_RefreshTokens_Family");

            entity.HasIndex(e => e.TokenHash, "UQ__RefreshT__BCB33F9277AA333D").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.TokenHash)
                .HasMaxLength(64)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RefreshTo__UserI__4F7CD00D");
        });

        modelBuilder.Entity<ReportDefinition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ReportDe__3214EC07045B25AB");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_ReportDefinitions_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_ReportDefinitions_TenantId_Id").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Tenant).WithMany(p => p.ReportDefinitions)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ReportDef__Tenan__7720AD13");
        });

        modelBuilder.Entity<ReportRun>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ReportRu__3214EC075AEB7B8D");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_ReportRuns_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_ReportRuns_TenantId_Id").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.StorageKey).HasMaxLength(450);

            entity.HasOne(d => d.Tenant).WithMany(p => p.ReportRuns)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ReportRun__Tenan__7EC1CEDB");

            entity.HasOne(d => d.ReportDefinition).WithMany(p => p.ReportRuns)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.DefinitionId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ReportRuns__7FB5F314");
        });

        modelBuilder.Entity<RobotsSnapshot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RobotsSn__3214EC07113CF0AD");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_RobotsSnapshots_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_RobotsSnapshots_TenantId_Id").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Tenant).WithMany(p => p.RobotsSnapshots)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RobotsSna__Tenan__44CA3770");

            entity.HasOne(d => d.Website).WithMany(p => p.RobotsSnapshots)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.WebsiteId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RobotsSnapshots_Websites");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Name).HasName("PK__Roles__737584F7D118FBAD");

            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.HasMany(d => d.Permissions).WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "RolePermission",
                    r => r.HasOne<Permission>().WithMany()
                        .HasForeignKey("Permission")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__RolePermi__Permi__68487DD7"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("Role")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__RolePermis__Role__6754599E"),
                    j =>
                    {
                        j.HasKey("Role", "Permission").HasName("PK__RolePerm__654932B1D7F23A5F");
                        j.ToTable("RolePermissions");
                        j.IndexerProperty<string>("Role")
                            .HasMaxLength(30)
                            .IsUnicode(false);
                        j.IndexerProperty<string>("Permission")
                            .HasMaxLength(100)
                            .IsUnicode(false);
                    });
        });

        modelBuilder.Entity<ScheduledJob>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Schedule__3214EC070FE3577A");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_ScheduledJobs_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_ScheduledJobs_TenantId_Id").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.CronExpression)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsDryRun).HasDefaultValue(true);
            entity.Property(e => e.Kind)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Tenant).WithMany(p => p.ScheduledJobs)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Scheduled__Tenan__67DE6983");

            entity.HasOne(d => d.Website).WithMany(p => p.ScheduledJobs)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.WebsiteId })
                .HasConstraintName("FK__ScheduledJobs__6ABAD62E");
        });

        modelBuilder.Entity<SchemaVersion>(entity =>
        {
            entity.HasKey(e => e.Version).HasName("PK__SchemaVe__0F540135F476A942");

            entity.Property(e => e.Version).ValueGeneratedNever();
            entity.Property(e => e.AppliedUtc).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<SearchConsoleMetric>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SearchCo__3214EC07121A1940");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_SearchConsoleMetrics_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_SearchConsoleMetrics_TenantId_Id").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AveragePosition).HasColumnType("decimal(12, 4)");
            entity.Property(e => e.Country)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Ctr).HasColumnType("decimal(12, 8)");
            entity.Property(e => e.Device)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PageUrl).HasMaxLength(2048);
            entity.Property(e => e.Query).HasMaxLength(1000);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Tenant).WithMany(p => p.SearchConsoleMetrics)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SearchCon__Tenan__634EBE90");

            entity.HasOne(d => d.Website).WithMany(p => p.SearchConsoleMetrics)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.WebsiteId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SearchConsoleMetrics_Websites");
        });

        modelBuilder.Entity<SeoFinding>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SeoFindi__3214EC07FACE33C7");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_SeoFindings_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_SeoFindings_TenantId_Id").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.RuleCode)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.RuleCodeNavigation).WithMany(p => p.SeoFindings)
                .HasForeignKey(d => d.RuleCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SeoFindin__RuleC__2DE6D218");

            entity.HasOne(d => d.Tenant).WithMany(p => p.SeoFindings)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SeoFindin__Tenan__2CF2ADDF");

            entity.HasOne(d => d.CrawledPage).WithMany(p => p.SeoFindings)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.PageId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SeoFindings__2FCF1A8A");

            entity.HasOne(d => d.Website).WithMany(p => p.SeoFindings)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.WebsiteId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SeoFindings_Websites");
        });

        modelBuilder.Entity<SeoRule>(entity =>
        {
            entity.HasKey(e => e.Code).HasName("PK__SeoRules__A25C5AA6CA328A2F");

            entity.Property(e => e.Code)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsEnabled).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Severity)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Sitemap>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Sitemaps__3214EC079CF442F0");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_Sitemaps_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_Sitemaps_TenantId_Id").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ContentHash)
                .HasMaxLength(32)
                .IsFixedLength();
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Url).HasMaxLength(2048);

            entity.HasOne(d => d.Tenant).WithMany(p => p.Sitemaps)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Sitemaps__Tenant__3E1D39E1");

            entity.HasOne(d => d.Website).WithMany(p => p.Sitemaps)
                .HasPrincipalKey(p => new { p.TenantId, p.Id })
                .HasForeignKey(d => new { d.TenantId, d.WebsiteId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sitemaps_Websites");
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subscrip__3214EC07A0D2CC97");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_Subscriptions_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_Subscriptions_TenantId_Id").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.BillingReference).HasMaxLength(200);
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Plan).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.PlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Subscript__PlanI__0FEC5ADD");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Subscript__Tenan__0EF836A4");
        });

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tenants__3214EC07E0C7FC92");

            entity.HasIndex(e => e.Slug, "UX_Tenants_Slug")
                .IsUnique()
                .HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Name).HasMaxLength(160);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Slug)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TenantMembership>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.UserId }).HasName("PK__TenantMe__FFE3CB25F2C1844C");

            entity.HasIndex(e => new { e.UserId, e.IsActive }, "IX_Memberships_User");

            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Role)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.RoleNavigation).WithMany(p => p.TenantMemberships)
                .HasForeignKey(d => d.Role)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TenantMemb__Role__48CFD27E");

            entity.HasOne(d => d.Tenant).WithMany(p => p.TenantMemberships)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TenantMem__Tenan__46E78A0C");

            entity.HasOne(d => d.User).WithMany(p => p.TenantMemberships)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TenantMem__UserI__47DBAE45");
        });

        modelBuilder.Entity<UsageMeter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UsageMet__3214EC073C87C04F");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_UsageMeters_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_UsageMeters_TenantId_Id").IsUnique();

            entity.HasIndex(e => new { e.TenantId, e.Metric, e.PeriodStart }, "UX_UsageMeters_Period").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Metric)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Tenant).WithMany(p => p.UsageMeters)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UsageMete__Tenan__1699586C");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC0720AC8C70");

            entity.HasIndex(e => e.NormalizedEmail, "UQ__Users__368B291A6C0A0CDD").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.SecurityStamp).HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<Website>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Websites__3214EC07B8191AAD");

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_Websites_TenantId_Id").IsUnique();

            entity.HasIndex(e => new { e.TenantId, e.CanonicalHost }, "UX_Websites_Tenant_Host")
                .IsUnique()
                .HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.BaseUrl).HasMaxLength(450);
            entity.Property(e => e.CanonicalHost).HasMaxLength(253);
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Name).HasMaxLength(160);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Tenant).WithMany(p => p.Websites)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Websites__Tenant__6D0D32F4");
        });

        modelBuilder.Entity<WebsiteSetting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WebsiteS__3214EC0763618710");

            entity.HasIndex(e => new { e.TenantId, e.CreatedUtc }, "IX_WebsiteSettings_Tenant_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.TenantId, e.Id }, "UQ_WebsiteSettings_TenantId_Id").IsUnique();

            entity.HasIndex(e => new { e.TenantId, e.WebsiteId }, "UX_WebsiteSettings_Website").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Environment)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Tenant).WithMany(p => p.WebsiteSettings)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WebsiteSe__Tenan__7C4F7684");

            entity.HasOne(d => d.Website).WithOne(p => p.WebsiteSetting)
                .HasPrincipalKey<Website>(p => new { p.TenantId, p.Id })
                .HasForeignKey<WebsiteSetting>(d => new { d.TenantId, d.WebsiteId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WebsiteSettings_Websites");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
