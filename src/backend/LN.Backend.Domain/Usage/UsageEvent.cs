namespace LN.Backend.Domain.Usage;

/// <summary>
/// Evento facturable registrado por el backend para el cobro pospago.
/// La unidad de cobro (documento / imagen / llamada) queda por confirmar en diseño;
/// se modela como un tipo de evento para no atarse a una sola.
/// </summary>
public sealed class UsageEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string UserKey { get; init; }
    public required string CompanyDb { get; init; }

    /// <summary>Tipo de evento facturable.</summary>
    public UsageEventType Type { get; init; }

    /// <summary>Cantidad de unidades (normalmente 1).</summary>
    public int Units { get; init; } = 1;

    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>Referencia opcional (ruta/ID de imagen, número de documento).</summary>
    public string? Reference { get; init; }
}

public enum UsageEventType
{
    VisionCall,       // llamada al modelo de visión
    ImageProcessed,   // imagen procesada
    DocumentCreated   // documento generado en SAP
}
