using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class SeoRule
{
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Severity { get; set; } = null!;

    public int Weight { get; set; }

    public bool IsEnabled { get; set; }

    public virtual ICollection<SeoFinding> SeoFindings { get; set; } = new List<SeoFinding>();
}
