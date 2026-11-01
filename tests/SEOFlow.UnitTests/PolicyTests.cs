using System.ComponentModel.DataAnnotations;
using SEOFlow.Contracts;
using SEOFlow.Domain;
using SEOFlow.Infrastructure;
using Xunit;
namespace SEOFlow.UnitTests;
public sealed class PolicyTests
{
    [Theory]
    [InlineData("AgencyAdmin", true, true)]
    [InlineData("SuperAdmin", true, true)]
    [InlineData("SEOManager", false, true)]
    [InlineData("Editor", false, false)]
    [InlineData("Analyst", false, false)]
    [InlineData("ClientViewer", false, false)]
    [InlineData("Unknown", false, false)]
    public void RolesAreLeastPrivilege(string role, bool manage, bool write) { Assert.Equal(manage, TenantPolicy.CanManage(role)); Assert.Equal(write, TenantPolicy.CanWriteWebsite(role)); }
    [Theory]
    [InlineData("EXAMPLE.COM", "https://example.com/")]
    [InlineData("https://Example.Com/", "https://example.com/")]
    public void DomainsNormalize(string input, string expected) => Assert.Equal(expected, DomainName.Normalize(input).AbsoluteUri);
    [Theory]
    [InlineData("http://example.com")]
    [InlineData("https://127.0.0.1")]
    [InlineData("https://localhost")]
    [InlineData("https://user:pass@example.com")]
    [InlineData("https://example.com/path")]
    [InlineData("https://example.com:444")]
    [InlineData("https://a.internal")]
    public void InvalidDomainsFail(string input) => Assert.Throws<ArgumentException>(() => DomainName.Normalize(input));
    [Fact] public void RandomTokensHaveSufficientEntropyAndAreHashed() { var a = AuthService.NewToken(); var b = AuthService.NewToken(); Assert.NotEqual(a, b); Assert.Equal(96, a.Length); Assert.Equal(64, AuthService.Hash(a).Length); Assert.NotEqual(a, AuthService.Hash(a)); }
    [Fact]
    public void PasswordHashesAreSaltedAndVerifyOnlyCorrectPassword()
    {
        var user = new SEOFlow.Infrastructure.Models.User();
        var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<SEOFlow.Infrastructure.Models.User>();
        var a = hasher.HashPassword(user, "A-strong-test-password-123"); var b = hasher.HashPassword(user, "A-strong-test-password-123");
        Assert.NotEqual(a, b);
        Assert.NotEqual(Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed, hasher.VerifyHashedPassword(user, a, "A-strong-test-password-123"));
        Assert.Equal(Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed, hasher.VerifyHashedPassword(user, a, "Wrong-password"));
    }
}
