using LN.Backend.Domain.Vision;
using LN.Backend.Infrastructure.Vision;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Proveedor de visión: stub en Fase 1; se sustituye por OpenAI/Azure OpenAI en Fase 3.
builder.Services.AddSingleton<IVisionProvider, StubVisionProvider>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Endpoint de salud (Fase 1).
app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "LN.Backend.Api" }));

// Esqueleto del proxy de IA: valida cuota (pendiente), llama al proveedor y registra consumo (pendiente).
app.MapPost("/api/vision/interpret", async (VisionRequest request, IVisionProvider vision, CancellationToken ct) =>
{
    // TODO (Fase 3/5): validar cuota del perfil y registrar UsageEvent antes/después de la llamada.
    var result = await vision.InterpretAsync(request, ct);
    return result.Success ? Results.Ok(result) : Results.BadRequest(result);
});

// Esqueleto de licencias (Fase 5).
app.MapGet("/api/licenses/{userKey}", (string userKey) =>
    Results.Ok(new { profile = "Basico", quotaTotal = 50, quotaUsed = 0, active = true }));

app.Run();
