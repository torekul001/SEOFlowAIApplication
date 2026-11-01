using SEOFlow.Contracts;
namespace SEOFlow.Web.Models;
public sealed record DashboardModel(IReadOnlyList<TenantView> Tenants, Guid? SelectedTenant, DashboardView? Dashboard, PageResult<WebsiteView>? Websites);
