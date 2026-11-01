using System;
using System.Collections.Generic;

namespace SEOFlow.Infrastructure.Models;

public partial class SchemaVersion
{
    public int Version { get; set; }

    public DateTime AppliedUtc { get; set; }
}
