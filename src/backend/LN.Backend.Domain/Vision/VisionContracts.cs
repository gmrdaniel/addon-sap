namespace LN.Backend.Domain.Vision;

/// <summary>Petición de interpretación de una imagen enviada por el add-on.</summary>
public sealed class VisionRequest
{
    public required string UserKey { get; init; }
    public required string CompanyDb { get; init; }

    /// <summary>Imagen en base64.</summary>
    public required string ImageBase64 { get; init; }

    /// <summary>Instrucciones del modelo (personalidad), tomadas de @LN_PERSONA.</summary>
    public string? SystemPrompt { get; init; }
}

/// <summary>Interpretación estructurada devuelta al add-on.</summary>
public sealed class VisionResult
{
    public bool Success { get; init; }
    public string? RawJson { get; init; }
    public string? Error { get; init; }

    /// <summary>Confianza global de la interpretación (0..1), si el modelo la provee.</summary>
    public double? Confidence { get; init; }
}

/// <summary>Abstracción del proveedor de IA de visión (OpenAI / Azure OpenAI).</summary>
public interface IVisionProvider
{
    Task<VisionResult> InterpretAsync(VisionRequest request, CancellationToken ct = default);
}
