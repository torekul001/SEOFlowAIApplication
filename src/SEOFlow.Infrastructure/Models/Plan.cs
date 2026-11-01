using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class Plan
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public int SiteLimit { get; set; }

    public int UserLimit { get; set; }

    public int PagesPerCrawl { get; set; }

    public int AiCredits { get; set; }

    public string FeatureFlagsJson { get; set; } = null!;

    public DateTime CreatedUtc { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
