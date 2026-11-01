namespace SEOFlow.Domain;
public static class TenantPolicy
{
    public static readonly string[] Roles = ["SuperAdmin", "AgencyAdmin", "SEOManager", "Editor", "Analyst", "ClientViewer"];
    public static bool CanManage(string role) => role is "SuperAdmin" or "AgencyAdmin";
    public static bool CanWriteWebsite(string role) => CanManage(role) || role == "SEOManager";
}
public static class DomainName
{
    public static Uri Normalize(string input)
    {
        if (!Uri.TryCreate(input.Contains("://") ? input : "https://" + input, UriKind.Absolute, out var uri)
            || uri.Scheme != "https" || !string.IsNullOrEmpty(uri.UserInfo) || uri.HostNameType != UriHostNameType.Dns
            || uri.Host.IndexOf('.') < 0 || !uri.IsDefaultPort || uri.AbsolutePath != "/" || uri.Query.Length > 0 || uri.Fragment.Length > 0)
            throw new ArgumentException("Enter a public HTTPS domain without a path, port, or credentials.");
        var host = uri.IdnHost.ToLowerInvariant().TrimEnd('.');
        if (host.EndsWith(".localhost") || host.EndsWith(".local") || host.EndsWith(".internal"))
            throw new ArgumentException("Internal domains are not supported.");
        return new Uri("https://" + host + "/");
    }
}
