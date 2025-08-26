namespace ReportHub.Common.Authentication;

public class JwtOptions
{
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public string SecretKey { get; init; }
    public string RefreshTokenSecretKey { get; set; }
    public int RefreshTokenExpirationMinutes { get; set; }


}
