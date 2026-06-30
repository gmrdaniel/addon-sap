namespace LN.Backend.Domain.Profiles;

/// <summary>Perfiles de licencia del add-on (cobro pospago).</summary>
public enum LicenseProfile
{
    Basico,
    Pro,
    Enterprise
}

/// <summary>Asignación de licencia y cuota a un usuario/empresa.</summary>
public sealed class License
{
    public required string UserKey { get; init; }
    public required string CompanyDb { get; init; }
    public LicenseProfile Profile { get; init; } = LicenseProfile.Basico;

    /// <summary>Cuota mensual de unidades facturables. 0 = sin tope (Enterprise).</summary>
    public int MonthlyQuota { get; init; }

    public bool Active { get; init; } = true;
}
