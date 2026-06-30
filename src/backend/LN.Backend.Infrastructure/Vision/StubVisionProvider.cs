using LN.Backend.Domain.Vision;

namespace LN.Backend.Infrastructure.Vision;

/// <summary>
/// Implementación temporal del proveedor de visión para el esqueleto de la Fase 1.
/// Se reemplaza por OpenAI/Azure OpenAI en la Fase 3, una vez confirmado el proveedor.
/// </summary>
public sealed class StubVisionProvider : IVisionProvider
{
    public Task<VisionResult> InterpretAsync(VisionRequest request, CancellationToken ct = default)
    {
        // Respuesta simulada con estructura estable para desarrollar el flujo del add-on.
        var result = new VisionResult
        {
            Success = true,
            Confidence = 0.0,
            RawJson = """
            { "cardCode": null, "items": [], "_stub": true }
            """
        };

        return Task.FromResult(result);
    }
}
