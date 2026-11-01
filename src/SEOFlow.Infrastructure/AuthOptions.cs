namespace SEOFlow.Infrastructure;
public sealed class AuthOptions
{
    public string SigningKey { get; set; } = "";
    public string Issuer { get; set; } = "SEOFlowAI";
    public string Audience { get; set; } = "SEOFlowAI";
    public bool RequireVerifiedEmail { get; set; } = true;
}
