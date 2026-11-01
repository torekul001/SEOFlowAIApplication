using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class Tenant
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public DateTime CreatedUtc { get; set; }

    public bool IsDeleted { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual ICollection<AiJob> AiJobs { get; set; } = new List<AiJob>();

    public virtual ICollection<AiPrompt> AiPrompts { get; set; } = new List<AiPrompt>();

    public virtual ICollection<AiUsage> AiUsages { get; set; } = new List<AiUsage>();

    public virtual ICollection<AnalyticsMetric> AnalyticsMetrics { get; set; } = new List<AnalyticsMetric>();

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<BacklinkSnapshot> BacklinkSnapshots { get; set; } = new List<BacklinkSnapshot>();

    public virtual ICollection<Competitor> Competitors { get; set; } = new List<Competitor>();

    public virtual ICollection<ContentDraft> ContentDrafts { get; set; } = new List<ContentDraft>();

    public virtual ICollection<CrawlRun> CrawlRuns { get; set; } = new List<CrawlRun>();

    public virtual ICollection<CrawledPage> CrawledPages { get; set; } = new List<CrawledPage>();

    public virtual ICollection<DomainVerification> DomainVerifications { get; set; } = new List<DomainVerification>();

    public virtual ICollection<DraftApproval> DraftApprovals { get; set; } = new List<DraftApproval>();

    public virtual ICollection<DraftVersion> DraftVersions { get; set; } = new List<DraftVersion>();

    public virtual ICollection<FindingHistory> FindingHistories { get; set; } = new List<FindingHistory>();

    public virtual ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();

    public virtual ICollection<KeywordGroup> KeywordGroups { get; set; } = new List<KeywordGroup>();

    public virtual ICollection<KeywordRankSnapshot> KeywordRankSnapshots { get; set; } = new List<KeywordRankSnapshot>();

    public virtual ICollection<KeywordSuggestion> KeywordSuggestions { get; set; } = new List<KeywordSuggestion>();

    public virtual ICollection<Keyword> Keywords { get; set; } = new List<Keyword>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<PageLink> PageLinks { get; set; } = new List<PageLink>();

    public virtual ICollection<PageSpeedRun> PageSpeedRuns { get; set; } = new List<PageSpeedRun>();

    public virtual ICollection<ProviderConnection> ProviderConnections { get; set; } = new List<ProviderConnection>();

    public virtual ICollection<ProviderSyncRun> ProviderSyncRuns { get; set; } = new List<ProviderSyncRun>();

    public virtual ICollection<PublishBackup> PublishBackups { get; set; } = new List<PublishBackup>();

    public virtual ICollection<PublishJob> PublishJobs { get; set; } = new List<PublishJob>();

    public virtual ICollection<PublishingConnection> PublishingConnections { get; set; } = new List<PublishingConnection>();

    public virtual ICollection<ReportDefinition> ReportDefinitions { get; set; } = new List<ReportDefinition>();

    public virtual ICollection<ReportRun> ReportRuns { get; set; } = new List<ReportRun>();

    public virtual ICollection<RobotsSnapshot> RobotsSnapshots { get; set; } = new List<RobotsSnapshot>();

    public virtual ICollection<ScheduledJob> ScheduledJobs { get; set; } = new List<ScheduledJob>();

    public virtual ICollection<SearchConsoleMetric> SearchConsoleMetrics { get; set; } = new List<SearchConsoleMetric>();

    public virtual ICollection<SeoFinding> SeoFindings { get; set; } = new List<SeoFinding>();

    public virtual ICollection<Sitemap> Sitemaps { get; set; } = new List<Sitemap>();

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    public virtual ICollection<TenantMembership> TenantMemberships { get; set; } = new List<TenantMembership>();

    public virtual ICollection<UsageMeter> UsageMeters { get; set; } = new List<UsageMeter>();

    public virtual ICollection<WebsiteSetting> WebsiteSettings { get; set; } = new List<WebsiteSetting>();

    public virtual ICollection<Website> Websites { get; set; } = new List<Website>();
}
