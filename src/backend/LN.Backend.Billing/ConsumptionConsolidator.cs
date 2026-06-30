using LN.Backend.Domain.Usage;

namespace LN.Backend.Billing;

/// <summary>
/// Consolida los eventos de consumo de un periodo para el cobro pospago.
/// Genera el reporte por empresa y por usuario al cierre del mes.
/// </summary>
public sealed class ConsumptionConsolidator
{
    /// <summary>Agrupa los eventos por empresa+usuario y suma las unidades del periodo.</summary>
    public IReadOnlyList<ConsumptionSummary> Consolidate(IEnumerable<UsageEvent> events)
    {
        return events
            .GroupBy(e => new { e.CompanyDb, e.UserKey })
            .Select(g => new ConsumptionSummary
            {
                CompanyDb = g.Key.CompanyDb,
                UserKey = g.Key.UserKey,
                TotalUnits = g.Sum(e => e.Units),
                EventCount = g.Count()
            })
            .OrderBy(s => s.CompanyDb)
            .ThenBy(s => s.UserKey)
            .ToList();
    }
}

/// <summary>Resumen de consumo de un usuario/empresa en un periodo.</summary>
public sealed class ConsumptionSummary
{
    public required string CompanyDb { get; init; }
    public required string UserKey { get; init; }
    public int TotalUnits { get; init; }
    public int EventCount { get; init; }
}
